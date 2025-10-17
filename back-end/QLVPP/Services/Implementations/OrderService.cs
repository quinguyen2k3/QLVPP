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

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                return false;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be deleted.");
            }

            order.IsActivated = false;

            await _unitOfWork.Order.Update(order);
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
                    inventory = new Inventory
                    {
                        ProductId = orderDetail.ProductId,
                        WarehouseId = request.WarehouseId,
                        Quantity = 0,
                    };
                    await _unitOfWork.Inventory.Add(inventory);
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
    }
}
