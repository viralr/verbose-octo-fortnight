using Microsoft.Extensions.Logging;
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
        static int workerId = 0;
        private IOrderData _data;
        static int orderIdInt = 0;
        static int orderItemIdInt = 0;
        private IWorkerData workerData;
        private ILogger<OrderService> _logger;
        public OrderService(IOrderData orderData, IWorkerData worker, ILogger<OrderService> logger)
        {
            _data = orderData;
            workerData = worker;
            _logger = logger;
        }
        public bool AddOrders(List<OrderResource> orders)
        {
            _logger.LogError("Test Error");
            _logger.LogWarning("Test Warning");
            orders.AsParallel().ForAll((o) =>
            {
                Order newOrder = ValidateOrderResourceAndAddToQueue(o);
                _data.AddOrder(newOrder);
            });
            
            return true;
        }

        public bool EnqueueOrder(Order order)
        {
            _data.AddOrder(order);
            return true;
        }

        public bool AddWorker(string userName, string password)
        {
            workerData.AddWorker(new Worker()
            {
                Id = workerId++,
                TaskRate = 0
            });
            return true;
        }

        public bool RemoveWorker(int id)
        {
            workerData.RemoveWorker(id);
            return true;
        }

        public List<OrderItem> GetWorkerTasks(int id)
        {
            return workerData.GetWorkerTasks(id);
        }

        public Order DequeueOrder()
        {
            if (workerData.AreWorkersAvailable())
                return _data.DequeueOrder();
            return null;
        }

        public List<Order> GetOrders(Status status)
        {
            return _data.GetOrders(status);
        }

        public bool MarkOrderItemAsComplete(int workerId, string itemId)
        {
            OrderItem orderItem = workerData.MarkOrderItemAsComplete(workerId, itemId);
            _data.PostProcessOrderItemComplete(orderItem);
            return true;
        }

        public void QueueOrderItemsForProcessing(Order order)
        {
            _data.UpdateOrderStatusToProcessing(order);
            workerData.QueueOrderItems(order.OrderItems.Values.ToList());
        }

        private Order ValidateOrderResourceAndAddToQueue(OrderResource order)
        {
            string orderId = orderIdInt++.ToString();
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

        private Dictionary<string, OrderItem> ValidateAndGetOrderItems(string orderId, List<OrderItemResource> items)
        {
            return items.Select(p => new OrderItem()
            {
                OrderId = orderId,
                OrderItemId = orderItemIdInt++.ToString(),
                Quanity = p.Quanity,
                Status = Status.Open
            }).ToDictionary(p => p.OrderItemId, p => p);
        }
    }
}
