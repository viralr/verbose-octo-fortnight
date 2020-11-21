using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Interfaces;
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
        public string GetOrderStatus(int id)
        {
            return "InProgress";
        }

        [HttpPost]
        public bool AddOrder([FromBody] OrderResource order)
        {
            return _orderService.AddOrder(order);
        }
    }
}
