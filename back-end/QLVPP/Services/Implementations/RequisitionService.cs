using AutoMapper;
using QLVPP.Constants;
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

        public RequisitionService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<RequisitionRes> Create(RequisitionReq request)
        {
            var requisition = _mapper.Map<Requisition>(request);
            requisition.Status = RequisitionStatus.Pending;

            await _unitOfWork.Requisition.Add(requisition);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<RequisitionRes>(requisition);
            return response;
        }

        public async Task<List<RequisitionRes>> GetAll()
        {
            var requisitions = await _unitOfWork.Requisition.GetAll();
            return _mapper.Map<List<RequisitionRes>>(requisitions);
        }

        public async Task<List<RequisitionRes>> GetAllActived()
        {
            var requisitions = await _unitOfWork.Requisition.GetAllIsActived();
            return _mapper.Map<List<RequisitionRes>>(requisitions);
        }

        public async Task<RequisitionRes?> GetById(long id)
        {
            var requisition = await _unitOfWork.Requisition.GetById(id);
            return requisition == null ? null : _mapper.Map<RequisitionRes>(requisition);
        }

        public async Task<RequisitionRes?> Update(long id, string status)
        {
            status = status.ToUpper();

            var requisition = await _unitOfWork.Requisition.GetById(id);
            if (requisition == null) return null;

            if (!RequisitionStatus.All.Contains(status))
            {
                throw new ArgumentException($"Invalid status: {status}");
            }

            if (requisition.Status is RequisitionStatus.Approved or RequisitionStatus.Rejected)
                throw new InvalidOperationException(
                    $"Requisition {id} is already {requisition.Status} and cannot be modified."
            );

            requisition.Status = status;

            if (status == RequisitionStatus.Approved)
            {
                requisition.ApprovedDate = DateTime.UtcNow;
                requisition.ApprovedBy = _currentUserService.UserAccount;
            }

            await _unitOfWork.Requisition.Update(requisition);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<RequisitionRes>(requisition);
        }
    }
}
