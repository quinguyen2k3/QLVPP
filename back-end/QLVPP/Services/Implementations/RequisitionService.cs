using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class RequisitionService : IRequisitionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public RequisitionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task Approve(ApproveReq request)
        {
            var employeeId = _currentUserService.GetUserId();

            var requisition = await _unitOfWork.Requisition.GetById(request.RequisitionId);
            if (requisition == null)
                throw new KeyNotFoundException($"Requisition #{request.RequisitionId} not found");

            if (requisition.Status != RequisitionStatus.Pending)
                throw new InvalidOperationException("Requisition is no longer pending approval");

            var approvalProcess = requisition.Process;
            var workflow = requisition.Config;

            var currentStepTasks = await _unitOfWork.ApprovalTask.GetByProcessIdAndConfigId(
                approvalProcess.Id,
                workflow.Id
            );

            if (!currentStepTasks.Any())
                throw new InvalidOperationException("No tasks found for the current step");

            var myTask = currentStepTasks.FirstOrDefault(t =>
                t.AssignedToId == employeeId || t.DelegateId == employeeId
            );

            if (myTask == null)
                throw new UnauthorizedAccessException(
                    "You are not allowed to approve this requisition"
                );

            if (workflow.ApprovalType == ApprovalType.SEQUENTIAL)
            {
                int nextSequence = currentStepTasks
                    .Where(t => t.Status == RequisitionStatus.Pending)
                    .Min(t => t.SequenceInGroup);

                if (myTask.SequenceInGroup != nextSequence)
                    throw new InvalidOperationException(
                        "It is not your turn to approve this requisition"
                    );
            }

            myTask.Status = RequisitionStatus.Approved;
            myTask.ApprovedDate = DateTime.UtcNow;
            myTask.ApprovedById = employeeId;
            myTask.Comments = request.Comments;

            await _unitOfWork.ApprovalTask.Update(myTask);

            int currentSequence = approvalProcess.CurrentStepOrder;

            if (workflow.ApprovalType == ApprovalType.SEQUENTIAL)
            {
                if (currentStepTasks.All(t => t.Status == RequisitionStatus.Approved))
                {
                    int maxSequence = currentStepTasks.Max(t => t.SequenceInGroup);
                    if (currentSequence < maxSequence)
                        approvalProcess.CurrentStepOrder++;
                    else
                        requisition.Status = RequisitionStatus.Approved;
                }
            }
            else if (workflow.ApprovalType == ApprovalType.PARALLEL)
            {
                int approvedCount = currentStepTasks.Count(t =>
                    t.Status == RequisitionStatus.Approved
                );
                int required = workflow.RequiredApprovals ?? currentStepTasks.Count;

                if (approvedCount >= required)
                {
                    int maxSequence = currentStepTasks.Max(t => t.SequenceInGroup);
                    if (currentSequence < maxSequence)
                        approvalProcess.CurrentStepOrder++;
                    else
                        requisition.Status = RequisitionStatus.Approved;
                }
            }

            await _unitOfWork.Requisition.Update(requisition);
            await _unitOfWork.ApprovalProcess.Update(approvalProcess);
            await _unitOfWork.SaveChanges();
        }

        public async Task<RequisitionRes> Create(RequisitionReq request)
        {
            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var productDict = (await _unitOfWork.Product.GetByIds(productIds)).ToDictionary(p =>
                p.Id
            );

            foreach (var productId in productIds)
            {
                if (!productDict.TryGetValue(productId, out var product))
                    throw new KeyNotFoundException($"Cannot find product #{productId}");

                if (!product.IsActivated)
                    throw new InvalidOperationException(
                        $"Product #{productId} has been deactivated"
                    );
            }

            var approverIds = request
                .Config.Approvers.Select(a => a.EmployeeId)
                .Distinct()
                .ToList();
            var approverDict = (await _unitOfWork.Employee.GetByIds(approverIds)).ToDictionary(a =>
                a.Id
            );

            foreach (var approverId in approverIds)
            {
                if (!approverDict.TryGetValue(approverId, out var employee))
                    throw new KeyNotFoundException($"Cannot find employee #{approverId}");

                if (!employee.IsActivated)
                    throw new InvalidOperationException(
                        $"Employee #{approverId} has been deactivated"
                    );
            }

            var requisition = _mapper.Map<Requisition>(request);
            requisition.Status = RequisitionStatus.Pending;
            requisition.RequesterId = _currentUserService.GetUserId();
            await _unitOfWork.Requisition.Add(requisition);

            var approvalConfig = requisition.Config;
            var approvalInstance = new ApprovalProcess
            {
                Requisition = requisition,
                CurrentStepOrder = 1,
                Status = RequisitionStatus.Pending,
            };
            await _unitOfWork.ApprovalProcess.Add(approvalInstance);

            int sequence = 1;
            foreach (var approverItem in approvalConfig.Approvers.OrderBy(a => a.Priority))
            {
                var stepInstance = new ApprovalTask
                {
                    ApprovalInstance = approvalInstance,
                    Config = approvalConfig,
                    AssignedToId = approverItem.EmployeeId,
                    ApprovalType = approvalConfig.ApprovalType,
                    SequenceInGroup = sequence++,
                    IsMandatory = approverItem.IsMandatory,
                    Status = RequisitionStatus.Pending,
                };

                await _unitOfWork.ApprovalTask.Add(stepInstance);
            }

            await _unitOfWork.SaveChanges();
            return _mapper.Map<RequisitionRes>(requisition);
        }

        public async Task<bool> Delete(long id)
        {
            var requisition = await _unitOfWork.Requisition.GetById(id);
            if (requisition == null)
            {
                return false;
            }

            if (requisition.Status != RequisitionStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot delete a requisition with status '{requisition.Status}'. Only pending requisitions can be deleted."
                );
            }

            requisition.IsActivated = false;
            requisition.Status = RequisitionStatus.Cancelled;

            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<List<RequisitionRes>> GetAllByMyself()
        {
            var curAccount = _currentUserService.GetUserAccount();
            var requisitions = await _unitOfWork.Requisition.GetByCreator(curAccount);
            return _mapper.Map<List<RequisitionRes>>(requisitions);
        }

        public async Task<RequisitionRes?> GetById(long id)
        {
            var requisition = await _unitOfWork.Requisition.GetById(id);
            return requisition == null ? null : _mapper.Map<RequisitionRes>(requisition);
        }

        public async Task<RequisitionRes?> Update(long id, string status)
        {
            var approverId = _currentUserService.GetUserId();

            status = status.ToUpper();

            if (status != RequisitionStatus.Approved && status != RequisitionStatus.Rejected)
            {
                throw new ArgumentException(
                    $"Invalid status update. Only '{RequisitionStatus.Approved}' or '{RequisitionStatus.Rejected}' are allowed."
                );
            }

            var requisition = await _unitOfWork.Requisition.GetById(id);
            if (requisition == null)
                return null;

            if (requisition.Status is RequisitionStatus.Approved or RequisitionStatus.Rejected)
            {
                throw new InvalidOperationException(
                    $"Requisition {id} is already {requisition.Status} and cannot be modified."
                );
            }

            await _unitOfWork.Requisition.Update(requisition);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<RequisitionRes>(requisition);
        }
    }
}
