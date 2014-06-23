using NUnit.Framework;

namespace Stretchr.Tests
{
    [TestFixture]
    internal class ClientTest
    {
        [Test]
        public void TestAt()
        {
            var client = new Client("account", "project", "key");
            RequestBuilder request = client.At("path");
            Assert.NotNull(request);
            Assert.AreEqual("path", request.Path);
        }

        [Test]
        public void TestNew()
        {
            var client = new Client("account", "project", "key");
            Assert.AreEqual("account", client.Account);
            Assert.AreEqual("project", client.Project);
            Assert.AreEqual("key", client.Key);
            Assert.AreEqual("account.stretchr.com", client.Host);
            Assert.AreEqual("1.1", client.ApiVersion);
            Assert.AreEqual("https", client.Protocol);
            Assert.NotNull(client.Transport);
            Assert.IsInstanceOf<HttpTransport>(client.Transport);

            ITransport transport = new FakeTransport();
            client = new Client("account", "project", "key", transport, "monkey.com", "1.0", "http");
            Assert.AreEqual("account", client.Account);
            Assert.AreEqual("project", client.Project);
            Assert.AreEqual("key", client.Key);
            Assert.AreEqual("monkey.com", client.Host);
            Assert.AreEqual("1.0", client.ApiVersion);
            Assert.AreEqual("http", client.Protocol);
            Assert.AreEqual(transport, client.Transport);
        }

        [Test]
        public void TestURLBase()
        {
            var client = new Client("account", "project", "key", host: "account.monkey.com", apiVersion: "1.0",
                protocol: "http");
            Assert.AreEqual("http://account.monkey.com/api/v1.0/project/", client.UrlBase());
        }
    }
}