using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static object TotalTimeLock = new object();

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
            Console.WriteLine($"Total Time = {TotalTime.TotalMilliseconds}");
            Console.WriteLine($"Min Time = {MinTime.TotalMilliseconds}");
            Console.WriteLine($"Max Time = {MaxTime.TotalMilliseconds}");
            Console.WriteLine($"Avg Time = {((tEnd - tStart)/PassedUrls).TotalMilliseconds}");
            Console.ReadLine();
        }

    }
}
