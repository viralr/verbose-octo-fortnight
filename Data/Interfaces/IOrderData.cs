using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Data.Interfaces
{
    public interface IOrderData
    {
        public void AddOrder(Order order);

        public Order DequeueOrder();

        public void UpdateOrderStatusToProcessing(Order order);
    }
}
