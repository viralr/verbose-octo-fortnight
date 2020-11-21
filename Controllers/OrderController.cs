using Microsoft.AspNetCore.Http;
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
    [Route("/order")]
    public class OrderController : Controller
    {
        private IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("all/{status}")]
        public List<Order> GetCompletedOrders(Status status)
        {
            return _orderService.GetOrders(status);
        }

        [HttpPost]
        [Route("add")]
        public bool AddOrder([FromBody] List<OrderResource> orders)
        {
            return _orderService.AddOrders(orders);
        }
    }
}
