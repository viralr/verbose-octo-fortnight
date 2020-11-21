using ServerApp.Models;
using ServerApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Interfaces
{
    public interface IOrderService
    {
        bool AddOrder(OrderResource order);
        Order DequeueOrder();
        void QueueOrderItemsForProcessing(Order order);
        bool EnqueueOrder(Order order);
        bool AddWorker(string userName, string password);
        bool RemoveWorker(int id);
        List<OrderItem> GetWorkerTasks(int id);
    }
}
