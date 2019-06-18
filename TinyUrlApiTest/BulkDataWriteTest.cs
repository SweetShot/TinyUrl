using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyUrlApi.Config;
using TinyUrlApi.Data;

namespace TinyUrlApiTest
{
    [TestClass]
    public class BulkDataWriteTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var repo = new TinyUrlBulkWriteRepository(new TinyUrlContext(new MongoDbConfig()
            {
                Database = "TinyUrlDB",
                Host = "localhost",
                Port = 27017,
                User = "root",
                Password = "example"
            }));

            TestContext.WriteLine("Done");
        }
    }
}
