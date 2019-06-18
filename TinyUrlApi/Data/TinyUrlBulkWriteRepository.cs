using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TinyUrlApi.Data
{
    public class TinyUrlBulkWriteRepository : ITinyUrlRepository
    {
        private readonly ITinyUrlContext _context;

        private readonly Tuple<List<WriteModel<MongoUrlEntity>>, List<EventWaitHandle>> _ops = 
            new Tuple<List<WriteModel<MongoUrlEntity>>, List<EventWaitHandle>>(new List<WriteModel<MongoUrlEntity>>(), 
                                                                               new List<EventWaitHandle>());

        private readonly Task _executionTask; 
        private readonly CancellationTokenSource _executionTaskCancellationTokenSource;
        public TinyUrlBulkWriteRepository(ITinyUrlContext context)
        {
            _context = context;
            _executionTaskCancellationTokenSource = new CancellationTokenSource();
            _executionTask = Task.Run(() => ExecutionTask(_executionTaskCancellationTokenSource.Token));
        }

        private void ExecutionTask(CancellationToken token)
        {
            while (true)
            {
                Task.Delay(100).Wait();
                if (_ops.Item1.Count == 0)
                {
                    continue;
                }

                WriteModel<MongoUrlEntity>[] insertList;
                EventWaitHandle[] eventList;
                lock (_ops)
                {
                    insertList = _ops.Item1.ToArray();
                    eventList = _ops.Item2.ToArray();
                    _ops.Item1.Clear();
                    _ops.Item2.Clear();
                }
                var ans = _context.TinyUrlCollection.BulkWrite(insertList);
                if (ans.InsertedCount != insertList.Length)
                {
                    throw new Exception("Insertion does not match expected");
                }
                foreach (var eventWait in eventList)
                {
                    eventWait.Set();
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        Task<MongoUrlEntity> AddInsertTask(MongoUrlEntity urlEntity)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            lock (_ops)
            {
                _ops.Item1.Add(new InsertOneModel<MongoUrlEntity>(urlEntity));
                _ops.Item2.Add(waitHandle);
            }

            var task = Task.Factory.StartNew(() =>
            {
                waitHandle.WaitOne();
                return urlEntity;
            });
            
            return task;
        }

        public MongoUrlEntity GetLongUrl(long shortUrlId)
        {
            return _context.TinyUrlCollection.Find(url => url.ShortUrlId == shortUrlId).FirstOrDefault();
        }

        public MongoUrlEntity PostLongUrl(MongoUrlEntity longMongoUrl)
        {
            var t = AddInsertTask(longMongoUrl);
            t.Wait();
            if (t.IsFaulted)
            {
                throw t.Exception ?? new Exception("Task faulted with empty exception.");
            }
            return t.Result;
        }

        public Task<MongoUrlEntity> GetLongUrlAsync(long shortUrlId)
        {
            return _context.TinyUrlCollection.Find(url => url.ShortUrlId == shortUrlId).FirstOrDefaultAsync();
        }

        public Task PostLongUrlAsync(MongoUrlEntity longMongoUrl)
        {
            return AddInsertTask(longMongoUrl);
        }

        ~TinyUrlBulkWriteRepository()
        {
            _executionTaskCancellationTokenSource.Cancel();
            _executionTask.Wait();
        }
        
    }
}
