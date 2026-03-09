using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class MaterialRequestService : IMaterialRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public MaterialRequestService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<MaterialRequestRes> Create(MaterialRequestReq request)
        {
            var approver = await _unitOfWork.Employee.GetById(request.ApproverId);
            var requester = await _unitOfWork.Employee.GetById(request.RequesterId);

            if (approver == null || requester == null)
            {
                throw new Exception("RequesterOrApproverNotFound");
            }

            if (approver.DepartmentId != requester.DepartmentId)
            {
                throw new Exception("ApproverMustBeInSameDepartment");
            }

            if (request.Type == RequestType.Return)
            {
                var stockOut = await _unitOfWork.StockOut.GetByCode(request.ReferenceId);

                if (stockOut == null)
                {
                    throw new Exception("ReferenceIdNotFound");
                }

                foreach (var item in request.Items)
                {
                    var issuedItem = stockOut.StockOutDetails.FirstOrDefault(x =>
                        x.ProductId == item.ProductId
                    );

                    if (issuedItem == null || item.Quantity > issuedItem.Quantity)
                    {
                        throw new Exception("ReturnQuantityExceedsIssuedQuantity");
                    }
                }
            }

            var materialRequest = _mapper.Map<MaterialRequest>(request);

            materialRequest.GenerateCode();

            materialRequest.Status = MaterialRequestStatus.Pending_Department;

            var initialLog = new ApprovalLog
            {
                MaterialRequest = materialRequest,
                ActorId = _currentUserService.GetUserId(),
                Action = "Created",
                Step = "Creator",
                Comment = request.Purpose,
                LogTime = DateTime.Now,
            };

            await _unitOfWork.MaterialRequest.Add(materialRequest);
            await _unitOfWork.ApprovalLog.Add(initialLog);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<MaterialRequestRes>(materialRequest);
        }

        public async Task<MaterialRequestRes> Update(long Id, MaterialRequestReq request)
        {
            var materialRequest = await _unitOfWork.MaterialRequest.GetById(Id);

            if (materialRequest == null)
            {
                throw new Exception("RequestNotFound");
            }

            if (materialRequest.CreatedBy != _currentUserService.GetUserAccount())
            {
                throw new UnauthorizedAccessException("NotAllowedToUpdate");
            }

            if (materialRequest.Status != MaterialRequestStatus.Pending_Department)
            {
                throw new InvalidOperationException("CannotUpdateInCurrentStatus");
            }

            var approver = await _unitOfWork.Employee.GetById(request.ApproverId);
            var requester = await _unitOfWork.Employee.GetById(request.RequesterId);

            if (approver == null || requester == null)
            {
                throw new Exception("RequesterOrApproverNotFound");
            }

            if (approver.DepartmentId != requester.DepartmentId)
            {
                throw new Exception("ApproverMustBeInSameDepartment");
            }

            if (request.Type == RequestType.Return)
            {
                var stockOut = await _unitOfWork.StockOut.GetByCode(request.ReferenceId);

                if (stockOut == null)
                {
                    throw new Exception("ReferenceIdNotFound");
                }

                foreach (var item in request.Items)
                {
                    var issuedItem = stockOut.StockOutDetails.FirstOrDefault(x =>
                        x.ProductId == item.ProductId
                    );

                    if (issuedItem == null || item.Quantity > issuedItem.Quantity)
                    {
                        throw new Exception("ReturnQuantityExceedsIssuedQuantity");
                    }
                }
            }

            _mapper.Map(request, materialRequest);

            await _unitOfWork.MaterialRequest.Update(materialRequest);

            var log = new ApprovalLog
            {
                MaterialRequest = materialRequest,
                ActorId = _currentUserService.GetUserId(),
                Action = "UpdatedRequest",
                Comment = "Updated details and quantity",
                LogTime = DateTime.Now,
            };

            await _unitOfWork.ApprovalLog.Add(log);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<MaterialRequestRes>(materialRequest);
        }

        public async Task<bool> Approve(ApproveReq request)
        {
            var materialRequest = await _unitOfWork.MaterialRequest.GetById(request.RequestId);

            if (materialRequest == null)
            {
                return false;
            }

            string nextStatus;
            string actionName;

            switch (materialRequest.Status)
            {
                case MaterialRequestStatus.Pending_Department:
                    nextStatus = MaterialRequestStatus.Pending_Warehouse;
                    actionName = "DepartmentApproved";
                    if (materialRequest.ApproverId != _currentUserService.GetUserId())
                    {
                        throw new UnauthorizedAccessException("NotAllowedToApprove");
                    }
                    break;

                case MaterialRequestStatus.Pending_Warehouse:
                    nextStatus = MaterialRequestStatus.Approved;
                    actionName = "WarehouseApproved";
                    if (
                        !await CheckUserRoleAsync(
                            _currentUserService.GetUserId(),
                            materialRequest.Status
                        )
                    )
                    {
                        throw new UnauthorizedAccessException("NotAllowedToApprove");
                    }
                    break;

                default:
                    throw new Exception("InvalidStatusForApproval");
            }

            materialRequest.Status = nextStatus;

            var log = new ApprovalLog
            {
                MaterialRequest = materialRequest,
                ActorId = _currentUserService.GetUserId(),
                Action = actionName,
                Comment = request.Comment,
                LogTime = DateTime.Now,
            };

            await _unitOfWork.ApprovalLog.Add(log);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Delegate(DelegateReq request)
        {
            var materialRequest = await _unitOfWork.MaterialRequest.GetById(request.RequestId);

            if (materialRequest == null)
            {
                throw new Exception("RequestNotFound");
            }

            if (materialRequest.ApproverId != _currentUserService.GetUserId())
            {
                throw new UnauthorizedAccessException("NotAllowedToDelegate");
            }

            if (materialRequest.RequesterId == request.DelegateToId)
            {
                throw new InvalidOperationException("CannotDelegateToRequester");
            }

            materialRequest.ApproverId = request.DelegateToId;

            var log = new ApprovalLog
            {
                MaterialRequest = materialRequest,
                ActorId = _currentUserService.GetUserId(),
                ToUserId = materialRequest.ApproverId,
                Action = "DelegatedTicket",
                Comment = request.Comments,
                LogTime = DateTime.Now,
            };

            await _unitOfWork.ApprovalLog.Add(log);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Reject(RejectReq request)
        {
            var materialRequest = await _unitOfWork.MaterialRequest.GetById(request.RequestId);

            if (materialRequest == null)
            {
                throw new Exception("RequestNotFound");
            }

            if (
                materialRequest.Status != MaterialRequestStatus.Pending_Department
                && materialRequest.Status != MaterialRequestStatus.Pending_Warehouse
            )
            {
                throw new InvalidOperationException("CannotRejectInCurrentStatus");
            }

            bool isAuthorized = false;

            if (materialRequest.Status == MaterialRequestStatus.Pending_Department)
            {
                isAuthorized = materialRequest.ApproverId == _currentUserService.GetUserId();
            }
            else if (materialRequest.Status == MaterialRequestStatus.Pending_Warehouse)
            {
                isAuthorized = await CheckUserRoleAsync(
                    _currentUserService.GetUserId(),
                    materialRequest.Status
                );
            }

            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException("NotAllowedToReject");
            }

            if (string.IsNullOrWhiteSpace(request.Comments))
            {
                throw new ArgumentException("RejectCommentsIsRequired");
            }

            materialRequest.Status = MaterialRequestStatus.Rejected;

            var log = new ApprovalLog
            {
                MaterialRequest = materialRequest,
                ActorId = _currentUserService.GetUserId(),
                Action = "Rejected",
                Comment = request.Comments,
                LogTime = DateTime.Now,
            };

            await _unitOfWork.ApprovalLog.Add(log);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Delete(long Id)
        {
            var materialRequest = await _unitOfWork.MaterialRequest.GetById(Id);

            if (materialRequest == null)
            {
                throw new Exception("RequestNotFound");
            }

            if (materialRequest.CreatedBy != _currentUserService.GetUserAccount())
            {
                throw new UnauthorizedAccessException("NotAllowedToDelete");
            }

            if (materialRequest.Status != MaterialRequestStatus.Pending_Department)
            {
                throw new InvalidOperationException("CannotDeleteInCurrentStatus");
            }

            materialRequest.IsActivated = false;
            await _unitOfWork.MaterialRequest.Update(materialRequest);
            var log = new ApprovalLog
            {
                MaterialRequest = materialRequest,
                ActorId = _currentUserService.GetUserId(),
                Action = "Delete",
                Comment = "Soft deleted the request",
                LogTime = DateTime.Now,
            };
            await _unitOfWork.ApprovalLog.Add(log);
            await _unitOfWork.SaveChanges();

            return true;
        }

        private async Task<bool> CheckUserRoleAsync(long userId, string currentStatus)
        {
            var user = await _unitOfWork.Employee.GetById(userId);

            if (user == null || user.Position == null)
            {
                return false;
            }

            switch (currentStatus)
            {
                case MaterialRequestStatus.Pending_Department:
                    return user.Position.Name.ToLower().Contains("Trưởng phòng");

                case MaterialRequestStatus.Pending_Warehouse:
                    return user.Position.Name.ToLower().Contains("Kho trưởng");

                default:
                    return false;
            }
        }

        public async Task<List<MaterialRequestRes>> GetByConditions(MaterialRequestFilterReq filter)
        {
            var entities = await _unitOfWork.MaterialRequest.GetByConditions(filter);
            return _mapper.Map<List<MaterialRequestRes>>(entities);
        }

        public async Task<MaterialRequestRes?> GetById(long Id)
        {
            var materialRequest = await _unitOfWork.MaterialRequest.GetById(Id);
            return materialRequest == null
                ? null
                : _mapper.Map<MaterialRequestRes>(materialRequest);
        }
    }
}
