using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Resources
{
    public class WorkerOrderResource
    {
        [Required]
        public int WorkerId { get; set; }
        [Required]
        public string ItemId { get; set; }
        [Required]
        public Status Status { get; set; }
    }
}
