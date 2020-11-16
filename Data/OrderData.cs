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
        private ConcurrentDictionary<long, Order> _processingOrders;
        private ConcurrentQueue<Order> _orderQueue;
        private ConcurrentQueue<Order> _orderItemQueue;
        private ConcurrentQueue<Order> _completedOrders;
        private ConcurrentDictionary<long, Order> _readyOrders;

        public OrderData()
        {
            _orderQueue = new ConcurrentQueue<Order>();
            _orderItemQueue = new ConcurrentQueue<Order>();
            _completedOrders = new ConcurrentQueue<Order>();
            _processingOrders = new ConcurrentDictionary<long, Order>();
            _readyOrders = new ConcurrentDictionary<long, Order>();
        }

        public async void AddOrder(Order order)
        {
            await new Task(() => _orderQueue.Enqueue(order));
        }

        public async Task<Order> DequeueOrder()
        {
            var task = Task<Order>.Factory.StartNew(() =>
           {
               if (_orderQueue.TryDequeue(out Order order))
               {
                   return order;
               }
               return null;
           });
            return await task;
        }
    }
}
