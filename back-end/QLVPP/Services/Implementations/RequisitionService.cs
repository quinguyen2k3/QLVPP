using AutoMapper;
using QLVPP.Constants.Status;
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

        public async Task<RequisitionRes> Create(RequisitionReq request)
        {
            var requisition = _mapper.Map<Requisition>(request);
            requisition.Status = RequisitionStatus.Pending;
            requisition.RequesterId = _currentUserService.GetUserId();

            await _unitOfWork.Requisition.Add(requisition);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<RequisitionRes>(requisition);
            return response;
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

            var step = requisition
                .ApprovalSteps.Where(s =>
                    s.AssignedToId == approverId && s.Status == RequisitionStatus.Pending
                )
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault();

            if (step == null)
                throw new InvalidOperationException(
                    "No pending approval step found for this approver."
                );

            step.Status = status;
            step.ApprovedAt = DateTime.Now;
            await _unitOfWork.ApprovalStep.Update(step);

            if (status == RequisitionStatus.Rejected)
            {
                requisition.Status = RequisitionStatus.Rejected;
            }
            else
            {
                if (requisition.ApprovalSteps.All(s => s.Status == RequisitionStatus.Approved))
                {
                    requisition.Status = RequisitionStatus.Approved;
                }
            }

            await _unitOfWork.Requisition.Update(requisition);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<RequisitionRes>(requisition);
        }
    }
}
