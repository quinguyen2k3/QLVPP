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

            var workflow = requisition.Config;

            var allTasks = await _unitOfWork.ApprovalTask.GetByConfigId(workflow.Id);

            if (!allTasks.Any())
                throw new InvalidOperationException("Approval tasks not found");

            var myTask = allTasks.FirstOrDefault(t =>
                (t.AssignedToId == employeeId || t.DelegateId == employeeId)
                && t.Status == RequisitionStatus.Pending
            );

            if (myTask == null)
                throw new UnauthorizedAccessException(
                    "You are not allowed to approve this requisition"
                );

            if (workflow.ApprovalType == ApprovalType.SEQUENTIAL)
            {
                int nextSeq = allTasks
                    .Where(t => t.Status == RequisitionStatus.Pending)
                    .Min(t => t.SequenceInGroup);

                if (myTask.SequenceInGroup != nextSeq)
                    throw new InvalidOperationException(
                        "It is not your turn to approve this requisition"
                    );
            }

            myTask.Status = RequisitionStatus.Approved;
            myTask.ApprovedDate = DateTime.UtcNow;
            myTask.ApprovedById = employeeId;
            myTask.Comments = request.Comments;

            await _unitOfWork.ApprovalTask.Update(myTask);

            var approvedTasks = allTasks.Where(t => t.Status == RequisitionStatus.Approved);

            bool isFullyApproved = false;

            if (workflow.ApprovalType == ApprovalType.SEQUENTIAL)
            {
                isFullyApproved = allTasks.All(t => t.Status == RequisitionStatus.Approved);
            }
            else if (workflow.ApprovalType == ApprovalType.PARALLEL)
            {
                int required = workflow.RequiredApprovals ?? allTasks.Count();
                isFullyApproved = approvedTasks.Count() >= required;
            }

            if (isFullyApproved)
                requisition.Status = RequisitionStatus.Approved;

            await _unitOfWork.Requisition.Update(requisition);
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
                if (!productDict.ContainsKey(productId))
                    throw new KeyNotFoundException($"Cannot find product #{productId}");

                if (!productDict[productId].IsActivated)
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
                if (!approverDict.ContainsKey(approverId))
                    throw new KeyNotFoundException($"Cannot find employee #{approverId}");

                if (!approverDict[approverId].IsActivated)
                    throw new InvalidOperationException(
                        $"Employee #{approverId} has been deactivated"
                    );
            }

            var requisition = _mapper.Map<Requisition>(request);
            requisition.Status = RequisitionStatus.Pending;
            requisition.RequesterId = _currentUserService.GetUserId();
            await _unitOfWork.Requisition.Add(requisition);

            int sequence = 1;
            foreach (var approverItem in request.Config.Approvers.OrderBy(x => x.Priority))
            {
                var task = new ApprovalTask
                {
                    Config = requisition.Config,
                    AssignedToId = approverItem.EmployeeId,
                    ApprovalType = requisition.Config.ApprovalType,
                    SequenceInGroup = sequence++,
                    IsMandatory = approverItem.IsMandatory,
                    Status = RequisitionStatus.Pending,
                };

                await _unitOfWork.ApprovalTask.Add(task);
            }

            await _unitOfWork.SaveChanges();
            return _mapper.Map<RequisitionRes>(requisition);
        }

        public async Task Delegate(DelegateReq request)
        {
            var userId = _currentUserService.GetUserId();

            var requisition =
                await _unitOfWork.Requisition.GetById(request.RequisitionId)
                ?? throw new KeyNotFoundException(
                    $"Requisition #{request.RequisitionId} not found"
                );

            if (requisition.Status != RequisitionStatus.Pending)
                throw new InvalidOperationException(
                    "Cannot delegate when requisition is not pending"
                );

            var delegateEmployee =
                await _unitOfWork.Employee.GetById(request.DelegateToEmployeeId)
                ?? throw new KeyNotFoundException(
                    $"Employee #{request.DelegateToEmployeeId} not found"
                );

            if (!delegateEmployee.IsActivated)
                throw new InvalidOperationException(
                    $"Employee #{request.DelegateToEmployeeId} is inactive"
                );

            var tasks = await _unitOfWork.ApprovalTask.GetByConfigId(requisition.Config.Id);

            var myTask =
                tasks.FirstOrDefault(t =>
                    t.AssignedToId == userId && t.Status == RequisitionStatus.Pending
                )
                ?? throw new InvalidOperationException(
                    "You have no pending approval task to delegate"
                );

            if (requisition.Config.ApprovalType == ApprovalType.SEQUENTIAL)
            {
                int nextSeq = tasks
                    .Where(t => t.Status == RequisitionStatus.Pending)
                    .Min(t => t.SequenceInGroup);

                if (myTask.SequenceInGroup != nextSeq)
                {
                    throw new InvalidOperationException(
                        $"You cannot delegate now because it is not your turn to approve. "
                            + $"Your sequence: {myTask.SequenceInGroup}, Current sequence: {nextSeq}"
                    );
                }
            }

            if (myTask.DelegateId != null)
                throw new InvalidOperationException(
                    "This approval task has already been delegated"
                );

            myTask.DelegateId = request.DelegateToEmployeeId;
            myTask.Comments = request.Comments;

            await _unitOfWork.ApprovalTask.Update(myTask);
            await _unitOfWork.SaveChanges();
        }

        public async Task Reject(RejectReq request)
        {
            var userId = _currentUserService.GetUserId();

            var requisition =
                await _unitOfWork.Requisition.GetById(request.RequisitionId)
                ?? throw new KeyNotFoundException(
                    $"Requisition #{request.RequisitionId} not found"
                );

            if (requisition.Status != RequisitionStatus.Pending)
                throw new InvalidOperationException("Requisition is not in pending state");

            var tasks = await _unitOfWork.ApprovalTask.GetByConfigId(requisition.Config.Id);

            var myTask =
                tasks.FirstOrDefault(t =>
                    (t.AssignedToId == userId || t.DelegateId == userId)
                    && t.Status == RequisitionStatus.Pending
                )
                ?? throw new UnauthorizedAccessException(
                    "You are not allowed to reject this requisition"
                );

            if (requisition.Config.ApprovalType == ApprovalType.SEQUENTIAL)
            {
                int nextSeq = tasks
                    .Where(t => t.Status == RequisitionStatus.Pending)
                    .Min(t => t.SequenceInGroup);

                if (myTask.SequenceInGroup != nextSeq)
                    throw new InvalidOperationException(
                        "It is not your turn to reject this requisition"
                    );
            }

            myTask.Status = RequisitionStatus.Rejected;
            myTask.ApprovedById = userId;
            myTask.ApprovedDate = DateTime.UtcNow;
            myTask.Comments = request.Comments;

            await _unitOfWork.ApprovalTask.Update(myTask);

            requisition.Status = RequisitionStatus.Rejected;

            await _unitOfWork.Requisition.Update(requisition);
            await _unitOfWork.SaveChanges();
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

        public async Task<List<RequisitionRes>> GetPendingRequisitionsForMe()
        {
            var employeeId = _currentUserService.GetUserId();

            var tasks = await _unitOfWork.ApprovalTask.GetPendingByEmployeeId(employeeId);

            var requisitionIds = tasks.Select(t => t.Config.RequisitionId).Distinct().ToList();

            var requisitions = await _unitOfWork.Requisition.GetByIds(requisitionIds);

            var result = _mapper.Map<List<RequisitionRes>>(requisitions);

            return result;
        }
    }
}
