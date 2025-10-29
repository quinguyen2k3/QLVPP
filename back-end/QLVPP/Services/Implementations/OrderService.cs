using AutoMapper;
using QLVPP.Constants;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<OrderRes> Create(OrderReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.OrderDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.OrderDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            if (request.Items != null && request.Items.Any())
            {
                var requestedProductIds = request
                    .Items.Select(item => item.ProductId)
                    .Distinct()
                    .ToList();

                var existingProducts = await _unitOfWork.Product.GetByIds(requestedProductIds);

                if (existingProducts.Count() != requestedProductIds.Count)
                {
                    var existingProductIds = existingProducts.Select(p => p.Id).ToHashSet();
                    var invalidProductIds = requestedProductIds.Where(id =>
                        !existingProductIds.Contains(id)
                    );

                    throw new InvalidOperationException(
                        $"One or more products do not exist. Invalid Product IDs: {string.Join(", ", invalidProductIds)}."
                    );
                }
            }
            var order = _mapper.Map<Order>(request);
            order.Status = OrderStatus.Pending;
            await _unitOfWork.Order.Add(order);

            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<OrderRes>(order);
            return response;
        }

        public async Task<List<OrderRes>> GetAll()
        {
            var requisitions = await _unitOfWork.Order.GetAll();
            return _mapper.Map<List<OrderRes>>(requisitions);
        }

        public async Task<List<OrderRes>> GetAllActivated()
        {
            var requisitions = await _unitOfWork.Order.GetAllIsActivated();
            return _mapper.Map<List<OrderRes>>(requisitions);
        }

        public async Task<OrderRes?> GetById(long id)
        {
            var order = await _unitOfWork.Order.GetById(id);
            return order == null ? null : _mapper.Map<OrderRes>(order);
        }

        public async Task<OrderRes?> Update(long id, OrderReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.OrderDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.OrderDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            if (request.Items != null && request.Items.Any())
            {
                var requestedProductIds = request
                    .Items.Select(item => item.ProductId)
                    .Distinct()
                    .ToList();

                var existingProducts = await _unitOfWork.Product.GetByIds(requestedProductIds);

                if (existingProducts.Count() != requestedProductIds.Count)
                {
                    var existingProductIds = existingProducts.Select(p => p.Id).ToHashSet();
                    var invalidProductIds = requestedProductIds.Where(id =>
                        !existingProductIds.Contains(id)
                    );

                    throw new InvalidOperationException(
                        $"One or more products do not exist. Invalid Product IDs: {string.Join(", ", invalidProductIds)}."
                    );
                }
            }

            var order = await _unitOfWork.Order.GetById(id);
            if (order == null)
                return null;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Order is not in Pending status and cannot be updated."
                );
            }

            _mapper.Map(request, order);

            await _unitOfWork.Order.Update(order);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<OrderRes>(order);
        }

        public async Task<bool> Delete(long id)
        {
            var order = await _unitOfWork.Order.GetById(id);
            if (order == null)
            {
                return false;
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException($"Order '{id}' has already been cancelled.");
            }

            DateOnly snapshotDate = (DateOnly)
                await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (today <= snapshotDate)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel the order because the inventory has been finalized for this period."
                );
            }

            switch (order.Status)
            {
                case OrderStatus.Pending:
                    break;

                case OrderStatus.Complete:
                    var orderDetails = order.OrderDetails;
                    if (orderDetails == null || !orderDetails.Any())
                    {
                        throw new InvalidOperationException(
                            $"Order '{id}' is complete but has no details to revert stock."
                        );
                    }

                    foreach (var detail in orderDetails)
                    {
                        var inventory = await _unitOfWork.Inventory.GetByKey(
                            order.WarehouseId,
                            detail.ProductId
                        );
                        if (inventory == null)
                        {
                            throw new InvalidOperationException(
                                $"Inventory record not found for Product ID '{detail.ProductId}' in Warehouse ID '{order.WarehouseId}'."
                            );
                        }

                        inventory.Quantity = inventory.Quantity - detail.Received;
                        await _unitOfWork.Inventory.Update(inventory);
                    }
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Cannot cancel an order with the status '{order.Status}'."
                    );
            }

            order.Status = OrderStatus.Cancelled;
            order.IsActivated = false;

            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<OrderRes?> Received(long id, OrderReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            if (latestSnapshotDate != null && request.OrderDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot receive items on date ({request.OrderDate:dd/MM/yyyy}) as it falls within a closed accounting period."
                );
            }

            var order = await _unitOfWork.Order.GetById(id);
            if (order == null)
            {
                return null;
            }

            if (order.Status == OrderStatus.Complete)
            {
                throw new InvalidOperationException("Order has already been completed.");
            }

            foreach (var item in request.Items)
            {
                if (item.Received <= 0)
                    continue;

                var orderDetail = order.OrderDetails.FirstOrDefault(d =>
                    d.ProductId == item.ProductId
                );
                if (orderDetail == null)
                {
                    throw new InvalidOperationException(
                        $"Product with ID {item.ProductId} not found in this order."
                    );
                }

                if (orderDetail.Received + item.Received > orderDetail.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Receiving quantity for product ID {item.ProductId} exceeds the ordered quantity. "
                            + $"Ordered: {orderDetail.Quantity}, Already Received: {orderDetail.Received}, Attempting to Receive: {item.Received}."
                    );
                }

                orderDetail.Received += item.Received;

                var inventory = await _unitOfWork.Inventory.GetByKey(
                    request.WarehouseId,
                    orderDetail.ProductId
                );
                if (inventory == null)
                {
                    throw new InvalidOperationException(
                        $"Inventory record not found for Product ID '{item.ProductId}' in Warehouse ID '{order.WarehouseId}'."
                    );
                }
                inventory.Quantity += item.Received;
                await _unitOfWork.Inventory.Update(inventory);
            }

            if (order.OrderDetails.All(d => d.Received >= d.Quantity))
            {
                order.Status = OrderStatus.Complete;
            }
            else if (order.OrderDetails.Any(d => d.Received > 0))
            {
                order.Status = OrderStatus.PartiallyReceived;
            }
            else
            {
                order.Status = OrderStatus.Pending;
            }

            await _unitOfWork.Order.Update(order);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<OrderRes>(order);
        }

        public async Task<List<OrderRes>> GetAllByMyself()
        {
            var curAccount = _currentUserService.GetUserAccount();
            var requisitions = await _unitOfWork.Order.GetByCreator(curAccount);
            return _mapper.Map<List<OrderRes>>(requisitions);
        }
    }
}
