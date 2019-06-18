using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyUrlApi.Helpers;

namespace TinyUrlApiTest
{
    [TestClass]
    public class DataConvertorTest
    {
        [TestMethod]
        public void TestConversion()
        {
            long a = 99111234;

            var ans = DataConvertor.ToBaseN(a);

            var b = DataConvertor.ToLongN(ans);

            Assert.AreEqual(a, b);

        }
    }
}
