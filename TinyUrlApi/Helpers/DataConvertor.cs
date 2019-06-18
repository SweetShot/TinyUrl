using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyUrlApi.Helpers
{
    public static class DataConvertor
    {
        private static string _base62chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string ToBaseN(long value, int baseN = 62)
        {
            if (baseN > 62)
            {
                throw new NotSupportedException();
            } 

            var ans = "";

            while (value > baseN)
            {
                ans = _base62chars[(int) value % baseN] + ans;
                value /= baseN;
            }

            ans = _base62chars[(int) value % baseN] + ans;
            return ans;
        }

        public static long ToLongN(string baseNStr, int baseN = 62)
        {
            long ans = 0;

            var i = 0;
            foreach(var c in baseNStr.Reverse())
            {
                ans += (long) Math.Pow(baseN, i) * _base62chars.IndexOf(c);
                i++;
            }
            return ans;
        }

    }
}
