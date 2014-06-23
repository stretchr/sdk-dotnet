using System.Collections.Generic;
using NUnit.Framework;

namespace Stretchr.Tests
{
    [TestFixture]
    internal class IntegrationTest
    {
        [Test]
        public void TestReadManyItems()
        {
            var stretchr = new Client("sandiego", "crime", "de0128c1cb7b70f583f56dd71da857df");
            DataResponse<Dictionary<string, dynamic>> response =
                stretchr.At("incidents").With("limit", "3").With("order", "~id").Read();
            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            IList<Dictionary<string, object>> items = response.Items();
            Assert.AreEqual(3, items.Count);

            IDictionary<string, object> item1 = items[0];
            Assert.AreEqual("8300  Block Jane Street", item1["address"]);

            IDictionary<string, object> item2 = items[1];
            Assert.AreEqual("4400  Block 32Nd Street", item2["address"]);

            IDictionary<string, object> item3 = items[2];
            Assert.AreEqual("800  Block Garnet Avenue", item3["address"]);
        }

        [Test]
        public void TestReadOneItem()
        {
            var stretchr = new Client("sandiego", "crime", "de0128c1cb7b70f583f56dd71da857df");
            DataResponse<Dictionary<string, dynamic>> response =
                stretchr.At("incidents").With("limit", "1").With("order", "~id").Read();
            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            IList<Dictionary<string, object>> items = response.Items();
            Assert.AreEqual(1, items.Count);

            IDictionary<string, object> item1 = items[0];
            Assert.AreEqual("8300  Block Jane Street", item1["address"]);
        }
    }
}