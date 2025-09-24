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

        public async Task<List<OrderRes>> GetAllActived()
        {
            var requisitions = await _unitOfWork.Order.GetAllIsActived();
            return _mapper.Map<List<OrderRes>>(requisitions);
        }

        public async Task<OrderRes?> GetById(long id)
        {
            var order = await _unitOfWork.Order.GetById(id);
            return order == null ? null : _mapper.Map<OrderRes>(order);
        }

        public async Task<OrderRes?> Update(long id, OrderReq request)
        {
            var order = await _unitOfWork.Order.GetById(id);
            if (order == null) return null;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Order is not in Pending status and cannot be updated.");
            }

            _mapper.Map(request, order);

            await _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<OrderRes>(order);
        }

        public async Task<bool> Delete(long id)
        {
            var order = await _unitOfWork.Order.GetById(id);
            if (order == null) return false;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be deleted.");
            }

            order.IsActived = false;

            await _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<OrderRes?> Received(long id, OrderReq request)
        {
            var order = await _unitOfWork.Order.GetById(id);
            if (order == null) return null;

            if (order.Status == OrderStatus.Complete)
            {
                throw new InvalidOperationException("Order has already been completed and cannot receive more items.");
            }

            foreach (var item in request.Items)
            {
                var detail = order.OrderDetails.FirstOrDefault(d => d.ProductId == item.ProductId);
                if (detail != null && item.Received > 0)
                {
                    detail.Received += item.Received;

                    var inventory = await _unitOfWork.Inventory
                        .GetByKey(request.WarehouseId, detail.ProductId);

                    if (inventory == null)
                    {
                        throw new InvalidOperationException(
                            $"Product {detail.ProductId} does not exist in warehouse {request.WarehouseId}."
                        );
                    }

                    inventory.Quantity += item.Received;
                    await _unitOfWork.Inventory.Update(inventory);
                }
            }
            if (order.OrderDetails.All(d => d.Received == 0))
                order.Status = OrderStatus.Pending;
            else if (order.OrderDetails.All(d => d.Received >= d.Quantity))
                order.Status = OrderStatus.Complete;
            else
                order.Status = OrderStatus.PartiallyReceived;

            await _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<OrderRes>(order);
        }
    }
}