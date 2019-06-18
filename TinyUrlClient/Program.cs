using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyUrlClient
{
    class Program
    {

        public static HttpClient Client = new HttpClient();

        public static TimeSpan TotalTime = TimeSpan.Zero;
        public static TimeSpan MinTime = TimeSpan.MaxValue;
        public static TimeSpan MaxTime = TimeSpan.MinValue;
        private static readonly object TotalTimeLock = new object();

        public static int TotalUrls = 0;
        public static int PassedUrls = 0;
        public static int FailedUrls = 0;


        static void Main(string[] args)
        {
            var tStart = DateTime.Now;
            var tasks = new List<Task>();
            using (var f = File.OpenText(@"F:\Visual Studio Projects\TinyUrl\TinyUrlApi\testurls.txt"))
            {
                tStart = DateTime.Now;
                while (!f.EndOfStream)
                {
                    var data = f.ReadLine()?.Split(" ");
                    var url = data[0];
                    var postdata = data[2];
                    Interlocked.Add(ref TotalUrls, 1);
                    Console.WriteLine($"Sending {TotalUrls}");
                    tasks.Add(Task.Run(() =>
                    {
                        var t1 = DateTime.Now;
                        var t = Client.PostAsync(new Uri(url), new StringContent($"\"{postdata}\"", Encoding.UTF8, "application/json"));
                        t.Wait();

                        if (t.IsFaulted)
                        {
                            Interlocked.Add(ref FailedUrls, 1);
                        }
                        else
                        {
                            Interlocked.Add(ref PassedUrls, 1);
                            var tdiff = DateTime.Now - t1;

                            // update 
                            lock (TotalTimeLock)
                            {
                                TotalTime += tdiff;
                                MinTime = (tdiff < MinTime) ? tdiff : MinTime;
                                MaxTime = (tdiff > MaxTime) ? tdiff : MaxTime;
                            }

                            Console.WriteLine($"Completed {PassedUrls}");
                        }
                    }));
                    Task.Delay(20).Wait();

                    if (TotalUrls > 10000)
                    {
                        break;
                    }
                }
            }

            Task.WaitAll(tasks.ToArray());
            var tEnd = DateTime.Now;

            // print results 
            Console.WriteLine($"Total = {TotalUrls} Passed = {PassedUrls} Failed = {FailedUrls}");
            Console.WriteLine($"Total Time Sync = {TotalTime.TotalMilliseconds}");
            Console.WriteLine($"Total Time = {(tEnd - tStart).TotalMilliseconds}");
            Console.WriteLine($"Min Time = {MinTime.TotalMilliseconds}");
            Console.WriteLine($"Max Time = {MaxTime.TotalMilliseconds}");
            Console.WriteLine($"One per = {((tEnd - tStart)/PassedUrls).TotalMilliseconds}");
            Console.WriteLine($"Avg Time = {((TotalTime)/PassedUrls).TotalMilliseconds}");
            Console.ReadLine();
        }

    }
}
/*
        Without bulking
        Total = 10001 Passed = 10001 Failed = 0
        Total Time Sync = 200,716,877.1593
        Total Time = 883,161.1669
        Min Time = 518.398
        Max Time = 78896.6492
        One per = 88.3073
        Avg Time = 20069.6807

        With bulk delay 50
        Total = 1001 Passed = 1001 Failed = 0
        Total Time Sync = 1,829,734.7728
        Total Time = 100447.5393
        Min Time = 291.6348
        Max Time = 4740.9761
        One per = 100.3472

        with bulk delay 10
        Total = 1001 Passed = 1001 Failed = 0
        Total Time Sync = 2,359,568.8125
        Total Time = 102396.0569
        Min Time = 46.9209
        Max Time = 6753.0643
        One per = 102.2938

        with bulk delay 100
        Total = 1001 Passed = 1001 Failed = 0
        Total Time Sync = 1,629,933.7027
        Total Time = 96,827.7823
        Min Time = 344.3136
        Max Time = 4851.5882
        One per = 96.7311
        Avg Time = 1628.3054

        with bulk delay 100
        Total = 10001 Passed = 10001 Failed = 0
        Total Time Sync = 13,007,127.1709
        Total Time = 923,177.312
        Min Time = 235.5273
        Max Time = 5429.6608
        One per = 92.3085
        Avg Time = 1300.5827
*/
