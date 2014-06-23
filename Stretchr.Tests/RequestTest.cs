using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;

namespace Stretchr.Tests
{
    [TestFixture]
    internal class RequestTest
    {
        [Test]
        public void TestNew()
        {
            var obj = new Dictionary<string, string>();
            var r = new Request("http://www.stretchr.com", HttpMethod.Get, obj);
            Assert.AreEqual("http://www.stretchr.com", r.Url);
            Assert.AreEqual(HttpMethod.Get, r.HttpMethod);
            Assert.AreEqual(obj, r.DataObject);
        }
    }
}