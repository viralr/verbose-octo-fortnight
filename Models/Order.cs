using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string IncomingOrderId { get; set; }
        public string Notes { get; set; }
        public Dictionary<string, OrderItem> OrderItems {get; set;}
        public Status Status { get; set; }
        public string OrderCallbackUrl { get; set; }
    }
}
