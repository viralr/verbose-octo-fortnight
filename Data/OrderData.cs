using ServerApp.Data.Interfaces;
using ServerApp.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Data
{
    public class OrderData : IOrderData
    {
        private ConcurrentDictionary<string, Order> _processingOrders;
        private ConcurrentQueue<Order> _orderQueue;
        private ConcurrentQueue<Order> _completedOrders;
        private ConcurrentDictionary<string, Order> _readyOrders;

        public OrderData()
        {
            _orderQueue = new ConcurrentQueue<Order>();
            _completedOrders = new ConcurrentQueue<Order>();
            _processingOrders = new ConcurrentDictionary<string, Order>();
            _readyOrders = new ConcurrentDictionary<string, Order>();
        }

        public async void AddOrder(Order order)
        {
            await Task.Factory.StartNew(() => {
                //Task.Delay(1000).Wait();
                _orderQueue.Enqueue(order);
                    
                });
        }

        public void UpdateOrderStatusToProcessing(Order order)
        {
            order.Status = Status.InProgress;
            _processingOrders.AddOrUpdate(order.OrderId, order, (key, oldValue) => order);
        }

        public Order DequeueOrder()
        {
            if (_orderQueue.TryDequeue(out Order order))
            {
                return order;
            }
            return null;
        }
    }
}
