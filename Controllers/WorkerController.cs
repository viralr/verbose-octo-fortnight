using Microsoft.AspNetCore.Mvc;
using ServerApp.Interfaces;
using ServerApp.Models;
using ServerApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("/worker")]
    public class WorkerController : Controller
    {
        private IOrderService _orderService;
        public WorkerController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Route("/login")]
        public void Login([FromBody] WorkerResource worker)
        {
            _orderService.AddWorker(worker.Username, worker.Password);
        }

        [HttpPost]
        [Route("/logout")]
        public void Logout(int id)
        {
            _orderService.RemoveWorker(id);
        }

        [HttpGet]
        [Route("/tasks")]
        public List<OrderItem> GetTasks(int id)
        {
            return _orderService.GetWorkerTasks(id);
        }

        [HttpPost]
        [Route("/tasks")]
        public List<OrderItem> MarkOrderItemAsComplete(int workerId, string orderItemId)
        {
            return _orderService.GetWorkerTasks(workerId);
        }
    }
}
