using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class OrderItem
    {
        public string OrderItemId { get; set; }
        public string OrderId { get; set; }
        public Status Status { get; set; }
        public ItemCategory Category {get; set;}
        public string Notes { get; set; }
        public int ItemIdentity { get; set; }
        public int Quanity { get; set; }
    }
}
