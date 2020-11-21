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
        private static ConcurrentDictionary<int, ConcurrentQueue<OrderItem>> workerToTaskQueue = new ConcurrentDictionary<int, ConcurrentQueue<OrderItem>>();
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, OrderItem>> workerToCurrentTask = 
            new ConcurrentDictionary<int, ConcurrentDictionary<string, OrderItem>>();
        private static ConcurrentDictionary<int, Worker> workers = new ConcurrentDictionary<int, Worker>();
        public WorkerData()
        {
        }

        public void AddWorker(Worker worker)
        {
            workers.AddOrUpdate(worker.Id, worker, (key, oldValue) => worker);
            workerToTaskQueue.AddOrUpdate(worker.Id, new ConcurrentQueue<OrderItem>(), (key, oldValue) => new ConcurrentQueue<OrderItem>());
            workerToCurrentTask.AddOrUpdate(worker.Id, new ConcurrentDictionary<string, OrderItem>(), 
                (key, oldValue) => new ConcurrentDictionary<string, OrderItem>());
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
                return workerToCurrentTask[workerId].Values.ToList();
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

                workerToCurrentTask[workerId].AddOrUpdate(orderItem.OrderItemId, orderItem, (key, oldValue) => orderItem);
                count++;
                if (count == 3)
                    return;
            }
        }

        public OrderItem MarkOrderItemAsComplete(int workerId, string itemId)
        {
            if (workers.ContainsKey(workerId))
            {
                workerToCurrentTask[workerId].TryRemove(itemId, out OrderItem item);
                if(item != null)
                {
                    return item;
                }
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
                            workerToCurrentTask.TryRemove(id, out ConcurrentDictionary<string, OrderItem> bag);
                            // dequeue work to other workers
                            if (queue != null && queue.Count > 0 || (bag != null && bag.Count > 0))
                            {
                                DistributeOrderItems(queue.ToList());
                                DistributeOrderItems(bag.Values.ToList());
                            }
                        }
                    }                    
                }
            });
        }

        public bool AreWorkersAvailable()
        {
            return workers.Count > 0;
        }

        private void DistributeOrderItems(List<OrderItem> items)
        {
            // distribute items across workers
            List<int> workerIds = workers.Keys.ToList();
            if (workerIds.Count == 0)
                throw new Exception("No workers available");
            int wIndex = new Random().Next(0, workerIds.Count), i = 0;
            while (i < items.Count)
            {                
                if (wIndex == workerIds.Count)
                {
                    wIndex = 0;
                }
                workerToTaskQueue[workerIds[wIndex]].Enqueue(items[i]);

                wIndex++;
                i++;
            }
        }
    }
}
