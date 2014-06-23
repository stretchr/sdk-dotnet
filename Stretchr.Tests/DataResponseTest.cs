using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Stretchr.Tests
{
    [TestFixture]
    public class DataResponseTest
    {
        [Test]
        public void TestErrorMessage()
        {
            var response = new DataResponse<TestResource>
            {
                Errors = new List<Error> {new Error {Message = "Something went wrong"}}
            };
            Assert.AreEqual("Something went wrong", response.ErrorMessage);
            response = new DataResponse<TestResource>();
            Assert.AreEqual(null, response.ErrorMessage);
        }

        [Test]
        public void TestItemsWithManyItems()
        {
            var response = new DataResponse<Dictionary<string, dynamic>>
            {
                Data = JsonConvert.DeserializeObject(@"{'~count':2,'~items':[{'name':'Tyler'},{'name':'Mat'}]}")
            };
            IList<Dictionary<string, object>> items = response.Items();
            Assert.NotNull(items);
            Assert.AreEqual(2, items.Count);
            IDictionary<string, object> tyler = items[0];
            IDictionary<string, object> mat = items[1];
            Assert.AreEqual("Tyler", tyler["name"]);
            Assert.AreEqual("Mat", mat["name"]);
        }

        [Test]
        public void TestItemsWithOneItem()
        {
            var response = new DataResponse<Dictionary<string, dynamic>>
            {
                Data = JsonConvert.DeserializeObject(@"{'name':'Tyler'}")
            };
            IList<Dictionary<string, object>> items = response.Items();
            Assert.NotNull(items);
            Assert.AreEqual(1, items.Count);
            IDictionary<string, object> tyler = items[0];
            Assert.AreEqual("Tyler", tyler["name"]);
        }
    }
}