using ServerApp.Data.Interfaces;
using ServerApp.Interfaces;
using ServerApp.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Data
{
    public class WorkerData : IWorkerData
    {        
        private object workerLock = new object();
        private ConcurrentDictionary<int, ConcurrentQueue<OrderItem>> workerToTaskQueue = new ConcurrentDictionary<int, ConcurrentQueue<OrderItem>>();
        private ConcurrentDictionary<int, Worker> workers = new ConcurrentDictionary<int, Worker>();
        private ConcurrentBag<int> currentOrderItems = new ConcurrentBag<int>();
        public WorkerData(IOrderService orderService)
        {

        }

        public void AddWorker(Worker worker)
        {
            workers.AddOrUpdate(worker.Id, worker, (key, oldValue) => worker);
            workerToTaskQueue.AddOrUpdate(worker.Id, new ConcurrentQueue<OrderItem>(), (key, oldValue) => new ConcurrentQueue<OrderItem>());
        }

        public OrderItem DequeueOrderItemForWorker(int workerId)
        {
            if(workers.ContainsKey(workerId))
            {
                workerToTaskQueue[workerId].TryDequeue(out OrderItem orderItem);
                return orderItem;
            }
            return null;
        }

        public void QueueOrderItems(List<OrderItem> items)
        {
            lock (workerLock)
            {
                // distribute items across workers
                List<int> workerIds = workers.Keys.ToList();
                int itemPerWorker = Math.Min(items.Count / workerIds.Count, 1);
                int wIndex = 0, i = 0, count = 0;
                while (i < items.Count)
                {
                    if (count == itemPerWorker)
                        wIndex++;
                    if (wIndex == workerIds.Count)
                    {
                        wIndex = 0;
                    }
                    workerToTaskQueue[workerIds[wIndex]].Enqueue(items[i]);

                    count++;
                    i++;
                }
            }                
        }

        public async void RemoveWorker(int id)
        {
            await Task.Factory.StartNew(() =>
            {
                if (workers.ContainsKey(id))
                {
                    lock (workerLock)
                    {
                        workers.TryRemove(id, out Worker worker);
                        if (worker != null)
                        {
                            workerToTaskQueue.TryRemove(id, out ConcurrentQueue<OrderItem> queue);
                            // dequeue work to other workers
                            if(queue != null && queue.Count > 0)
                            {

                            }
                        }
                    }                    
                }
            });
        }
    }
}
