using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Resources
{
    public class OrderResource
    {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string OrderCallbackUrl { get; set; }
        [Required]
        public List<OrderItemResource> OrderItems { get; set; }
        public string Notes { get; set; }
    }
}
