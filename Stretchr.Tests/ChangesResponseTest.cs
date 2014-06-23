using Newtonsoft.Json;
using NUnit.Framework;

namespace Stretchr.Tests
{
    [TestFixture]
    internal class ChangesResponseTest
    {
        [Test]
        public void TestChanges()
        {
            var response =
                JsonConvert.DeserializeObject<ChangesResponse>(
                    @"{'~changes':{'~updated':1,'~deleted':2,'~created':3,'~deltas':[{'~id':'id1','~created':123},{'~id':'id2','~updated':124}]},'~status':200}");

            Assert.AreEqual(1, response.Changes.Updated);
            Assert.AreEqual(2, response.Changes.Deleted);
            Assert.AreEqual(3, response.Changes.Created);
            Assert.AreEqual(2, response.Changes.Deltas.Count);
            Assert.AreEqual("id1", response.Changes.Deltas[0]["~id"]);
            Assert.AreEqual(123, response.Changes.Deltas[0]["~created"]);
            Assert.AreEqual("id2", response.Changes.Deltas[1]["~id"]);
            Assert.AreEqual(124, response.Changes.Deltas[1]["~updated"]);
        }
    }
}