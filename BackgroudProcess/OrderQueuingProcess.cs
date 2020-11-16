using Microsoft.Extensions.Hosting;
using ServerApp.Interfaces;
using ServerApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp.BackgroudProcess
{
    public class OrderQueuingProcess : IHostedService
    {
        private IOrderService _orderService;
        public OrderQueuingProcess(IOrderService orderService)
        {
            _orderService = orderService;
        }
        private Timer _timer;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(QueueOrder, null, TimeSpan.Zero,
                TimeSpan.FromMilliseconds(500));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void QueueOrder(object state)
        {
            // check if any workers are free before dequeing
            Order order = _orderService.DequeueOrder();
            if(order != null)
            {
                
            }
        }
    }
}
