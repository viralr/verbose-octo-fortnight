using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Resources
{
    public class OrderItemResource
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Quanity { get; set; }
        [Required]
        public int ItemIdentity { get; set; }
        public string Notes { get; set; }
    }
}
