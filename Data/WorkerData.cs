using ServerApp.Interfaces;
using ServerApp.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Data
{
    public class WorkerData
    {
        
        private object workerRemoveLock = new object();
        private ConcurrentDictionary<int, ConcurrentQueue<string>> workerToTaskQueue = new ConcurrentDictionary<int, ConcurrentQueue<string>>();
        private ConcurrentDictionary<int, Worker> workers = new ConcurrentDictionary<int, Worker>();
        private ConcurrentBag<int> currentOrderItems = new ConcurrentBag<int>();
        public WorkerData(IOrderService orderService)
        {

        }

        public void AddWorker(Worker worker)
        {
            workers.AddOrUpdate(worker.Id, worker, (key, oldValue) => worker);
            workerToTaskQueue.AddOrUpdate(worker.Id, new ConcurrentQueue<string>(), (key, oldValue) => new ConcurrentQueue<string>());
        }

        public async void RemoveWorker(int id)
        {
            await Task.Factory.StartNew(() =>
            {
                if (workers.ContainsKey(id))
                {
                    lock (workerRemoveLock)
                    {
                        workers.TryRemove(id, out Worker worker);
                        if (worker != null)
                        {
                            workerToTaskQueue.TryRemove(id, out ConcurrentQueue<string> queue);
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
