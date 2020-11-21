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
        private static object orderLock = new object();
        private static ConcurrentDictionary<string, Order> _orders;
        private static ConcurrentQueue<string> _orderQueue;
        private static ConcurrentQueue<string> _completedOrders;
        private static ConcurrentBag<string> _readyOrders;
        public OrderData()
        {
            _orderQueue = new ConcurrentQueue<string>();
            _completedOrders = new ConcurrentQueue<string>();
            _orders = new ConcurrentDictionary<string, Order>();
            _readyOrders = new ConcurrentBag<string>();
        }

        public async void AddOrder(Order order)
        {
            await Task.Factory.StartNew(() => {
                //Task.Delay(1000).Wait();
                _orders.TryAdd(order.OrderId, order);
                _orderQueue.Enqueue(order.OrderId);
                });
        }

        public void UpdateOrderStatusToProcessing(Order order)
        {
            order.Status = Status.InProgress;
            //_processingOrders.AddOrUpdate(order.OrderId, order, (key, oldValue) => order);
        }

        public void PostProcessOrderItemComplete(OrderItem item)
        {
            if(item.Status != Status.Complete)
            {
                lock(orderLock)
                {
                    item.Status = Status.Complete;
                    string orderId = item.OrderId;
                    if(_orders.TryGetValue(orderId, out Order order))
                    {
                        if(order.Status != Status.Complete)
                        {
                            if (order.OrderItems.Values.All(p => p.Status == Status.Complete))
                            {
                                order.Status = Status.Complete;
                                _completedOrders.Enqueue(orderId);
                            }                            
                        }                        
                    }
                }
            }
        }

        public List<Order> GetOrders(Status status)
        {
            return _orders.Values.Where(p => p.Status == status).ToList();
        }

        public void MarkOrderAsReady(string orderId)
        {
            lock (orderLock)
            {                
                if (_orders.TryGetValue(orderId, out Order order))
                {
                    order.Status = Status.Ready;
                    _readyOrders.Add(orderId);
                }
            }
        }

        public Order DequeueOrder()
        {
            if (_orderQueue.TryDequeue(out string orderId))
            {
                _orders.TryGetValue(orderId, out Order order);
                return order;
            }
            return null;
        }
    }
}
