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
        private static object workerLock = new object();
        private ConcurrentDictionary<int, ConcurrentQueue<OrderItem>> workerToTaskQueue = new ConcurrentDictionary<int, ConcurrentQueue<OrderItem>>();
        private ConcurrentDictionary<int, ConcurrentBag<OrderItem>> workerToCurrentTask = new ConcurrentDictionary<int, ConcurrentBag<OrderItem>>();
        private ConcurrentDictionary<int, Worker> workers = new ConcurrentDictionary<int, Worker>();
        private ConcurrentBag<int> currentOrderItems = new ConcurrentBag<int>();
        public WorkerData(IOrderService orderService)
        {

        }

        public void AddWorker(Worker worker)
        {
            workers.AddOrUpdate(worker.Id, worker, (key, oldValue) => worker);
            workerToTaskQueue.AddOrUpdate(worker.Id, new ConcurrentQueue<OrderItem>(), (key, oldValue) => new ConcurrentQueue<OrderItem>());
            workerToCurrentTask.AddOrUpdate(worker.Id, new ConcurrentBag<OrderItem>(), (key, oldValue) => new ConcurrentBag<OrderItem>());
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

        public List<OrderItem> GetWorkerTasks(int workerId)
        {
            if (workers.ContainsKey(workerId))
            {
                if(workerToCurrentTask[workerId].Count < 3)
                {
                    AddCurrentTasksToWorker(workerId);
                }
                return workerToCurrentTask[workerId].ToList();
            }
            return null;
        }

        private void AddCurrentTasksToWorker(int workerId)
        {
            int count = 0;
            while(true)
            {
                workerToTaskQueue[workerId].TryDequeue(out OrderItem orderItem);
                if (orderItem == null)
                    break;

                workerToCurrentTask[workerId].Add(orderItem);
                count++;
                if (count == 3)
                    return;
            }
        }

        public List<OrderItem> MarkOrderItemAsComplete(int workerId, string itemId)
        {
            if (workers.ContainsKey(workerId))
            {
                return workerToTaskQueue[workerId].ToList();
            }
            return null;
        }

        public void QueueOrderItems(List<OrderItem> items)
        {
            lock (workerLock)
            {
                DistributeOrderItems(items);
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
                            workerToCurrentTask.TryRemove(id, out ConcurrentBag<OrderItem> bag);
                            // dequeue work to other workers
                            if (queue != null && queue.Count > 0 || (bag != null && bag.Count > 0))
                            {
                                DistributeOrderItems(queue.ToList());
                                DistributeOrderItems(bag.ToList());
                            }
                        }
                    }                    
                }
            });
        }

        private void DistributeOrderItems(List<OrderItem> items)
        {
            // distribute items across workers
            List<int> workerIds = workers.Keys.ToList();
            if (workerIds.Count == 0)
                throw new Exception("No workers available");
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
}
