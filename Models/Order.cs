using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Notes { get; set; }
        public List<OrderItem> OrderItems {get; set;}
        public Status Status { get; set; }
        public string OrderCallbackUrl { get; set; }

    }
}
