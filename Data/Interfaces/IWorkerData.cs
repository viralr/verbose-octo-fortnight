﻿using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Data.Interfaces
{
    public interface IWorkerData
    {
        void QueueOrderItems(List<OrderItem> items);
    }
}
