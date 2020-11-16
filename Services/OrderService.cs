using ServerApp.Data.Interfaces;
using ServerApp.Interfaces;
using ServerApp.Models;
using ServerApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Services
{
    public class OrderService : IOrderService
    {
        private IOrderData _data;
        private IWorkerData workerData;
        public OrderService(IOrderData orderData, IWorkerData worker)
        {
            _data = orderData;
            workerData = worker;
        }
        public bool AddOrder(OrderResource order)
        {
            Order newOrder = ValidateOrderResourceAndAddToQueue(order);
            _data.AddOrder(newOrder);
            return true;
        }

        public Order DequeueOrder()
        {
            return _data.DequeueOrder();
        }

        public void QueueOrderItemsForProcessing(Order order)
        {
            _data.UpdateOrderStatusToProcessing(order);
            workerData.QueueOrderItems(order.OrderItems);
        }

        private Order ValidateOrderResourceAndAddToQueue(OrderResource order)
        {
            string orderId = Guid.NewGuid().ToString();
            Order newOrder = new Order()
            {
                OrderId = orderId,
                IncomingOrderId = order.OrderId,
                OrderCallbackUrl = order.OrderCallbackUrl,
                OrderItems = ValidateAndGetOrderItems(orderId, order.OrderItems),
                Notes = order.Notes,
                Status = Status.Open
            };


            return newOrder;
        }

        private List<OrderItem> ValidateAndGetOrderItems(string orderId, List<OrderItemResource> items)
        {
            return items.Select(p => new OrderItem()
            {
                OrderId = orderId,
                OrderItemId = Guid.NewGuid().ToString(),
                Status = Status.Open
            }).ToList();
        }
    }
}
