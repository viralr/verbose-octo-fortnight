using Microsoft.AspNetCore.Authorization;
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
        [Route("login")]
        public void Login([FromBody] WorkerResource worker)
        {
            _orderService.AddWorker(worker.Username, worker.Password);
        }

        [HttpPost]
        [Route("logout/{id}")]
        public void Logout(int id)
        {
            _orderService.RemoveWorker(id);
        }

        [HttpGet]
        [Route("tasks/{id}")]
        public List<OrderItem> GetTasks(int id)
        {
            return _orderService.GetWorkerTasks(id);
        }

        [HttpPut]
        [Route("tasks/update")]
        public bool MarkOrderItemAsComplete([FromBody] WorkerOrderResource workerOrderResource)
        {
            return _orderService.MarkOrderItemAsComplete(workerOrderResource.WorkerId, workerOrderResource.ItemId);
        }
    }
}
