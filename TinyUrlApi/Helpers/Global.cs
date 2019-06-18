using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TinyUrlApi.Helpers
{
    public class Global
    {
        public static int Prefix = 100; // we use 3 digit server unique name/int 

        private static long Counter = 176336; // 1476336 is minimum for 5 letter url name but we have 3 digit prefix so we use 76366 prefixed with 1

        private static object _counterLock = new object();

        public static void SetCounter(long val)
        {
            lock (_counterLock)
            {
                Counter = val;
            }
        }

        public static long GetCounter()
        {
            long ans;
            lock (_counterLock)
            {
                Counter += 1;
                ans = Counter;
            }

            return ans;
        }
        
    }
}
