using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Stretchr.Tests
{
    [TestFixture]
    internal class RequestBuilderTest
    {
        [SetUp]
        public void Setup()
        {
            _fakeTransport = new FakeTransport();
            _testClient = new Client("account", "project", "abc123", _fakeTransport);
        }

        private Client _testClient;
        private FakeTransport _fakeTransport;

        [Test]
        public void ParameterHelpers()
        {
            // WithTotal
            var request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.WithTotal());
            Assert.True(request.PathAndParams().Contains("total=1"));

            // Order
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Order("age"));
            Assert.True(request.PathAndParams().Contains("order=age"));

            // Limit
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Limit(2));
            Assert.True(request.PathAndParams().Contains("limit=2"));

            // skip
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Skip(20));
            Assert.True(request.PathAndParams().Contains("skip=20"));

            // paging
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Page(10, 1));
            Assert.True(request.PathAndParams().Contains("limit=10"));
            Assert.True(request.PathAndParams().Contains("skip=0"));
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Page(20, 2));

            Assert.True(request.PathAndParams().Contains("limit=20"));
            Assert.True(request.PathAndParams().Contains("skip=20"));
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Page(20, 1));
            Assert.True(request.PathAndParams().Contains("limit=20"));
            Assert.True(request.PathAndParams().Contains("skip=0"));

            // versioning
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.WithVersions());
            Assert.True(request.PathAndParams().Contains("versioning=1"));

            // aggregation
            request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.WithAggregation("group(field).count()"));
            Assert.True(request.PathAndParams().Contains("agg=group(field).count()"));
        }

        [Test]
        public void TestCreate()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var obj = new Dictionary<string, object>
            {
                {
                    "name",
                    "Tyler"
                }
            };

            _fakeTransport.Responses.Enqueue(new ChangesResponse());
            ChangesResponse response = rb.Create(obj);
            Request request = _fakeTransport.Requests.Dequeue();
            Assert.AreEqual(HttpMethod.Post.ToString(), request.HttpMethod.ToString());
            Assert.AreEqual(_testClient.UrlBase() + "people/1/books?key=abc123", request.Url);
            Assert.AreEqual(obj, request.DataObject);
        }

        [Test]
        public void TestCreateApplyChanges()
        {
            var mat = new TestResource {Name = "Mat"};

            // make a test response
            var r = new ChangesResponse
            {
                Status = HttpStatusCode.Created,
                Changes = new ChangeInfo
                {
                    Created = 1,
                    Deltas = new List<Dictionary<string, dynamic>>
                    {
                        new Dictionary<string, dynamic>
                        {
                            {"~id", "ABC123"},
                            {"something-else", "yes"}
                        }
                    }
                }
            };
            _fakeTransport.Responses.Enqueue(r);
            _testClient.At("people").Create(mat);

            Assert.AreEqual("Mat", mat.Name);
            Assert.AreEqual("ABC123", mat["~id"]);
            Assert.AreEqual("yes", mat["something-else"]);
        }

        [Test]
        public void TestCreateManyApplyChanges()
        {
            var mat = new TestResource {Name = "Mat"};
            var ryan = new TestResource {Name = "Ryan"};
            var tyler = new TestResource {Name = "Tyler"};

            // make a test response
            var r = new ChangesResponse
            {
                Status = HttpStatusCode.Created,
                Changes = new ChangeInfo
                {
                    Created = 1,
                    Deltas = new List<Dictionary<string, dynamic>>
                    {
                        new Dictionary<string, dynamic>
                        {
                            {"~id", "ABC123"},
                            {"something-else", "yes"}
                        },
                        new Dictionary<string, dynamic>
                        {
                            {"~id", "ABC124"},
                            {"something-else", "no"}
                        },
                        new Dictionary<string, dynamic>
                        {
                            {"~id", "ABC125"},
                            {"something-else", "maybe"}
                        }
                    }
                }
            };
            _fakeTransport.Responses.Enqueue(r);
            _testClient.At("people").Create(new List<TestResource> {mat, ryan, tyler});

            Assert.AreEqual("Mat", mat.Name);
            Assert.AreEqual("ABC123", mat.Id);
            Assert.AreEqual("yes", mat["something-else"]);

            Assert.AreEqual("Ryan", ryan.Name);
            Assert.AreEqual("ABC124", ryan.Id);
            Assert.AreEqual("no", ryan["something-else"]);

            Assert.AreEqual("Tyler", tyler.Name);
            Assert.AreEqual("ABC125", tyler.Id);
            Assert.AreEqual("maybe", tyler["something-else"]);
        }

        [Test]
        public void TestDelete()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            ChangesResponse response = rb.Delete();
            Request request = _fakeTransport.Requests.Dequeue();
            Assert.AreEqual(HttpMethod.Delete.ToString(), request.HttpMethod.ToString());
            Assert.AreEqual(_testClient.UrlBase() + "people/1/books?key=abc123", request.Url);
        }

        [Test]
        public void TestMustReadGeneric_Happy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<TestResource>
            {
                Status = HttpStatusCode.OK,
                Data = JsonConvert.DeserializeObject(@"{'name':'Mat'}")
            };
            _fakeTransport.Responses.Enqueue(response);
            var item = rb.MustReadOne<TestResource>();
            Assert.AreEqual("Mat", item.Name);
        }

        [Test]
        public void TestMustReadGeneric_Unhappy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<TestResource>
            {
                Status = HttpStatusCode.BadRequest,
                Errors = new List<Error> {new Error {Message = "Something went wrong"}}
            };
            _fakeTransport.Responses.Enqueue(response);
            Assert.Throws<ServerException>(() => rb.MustReadOne<TestResource>());
        }

        [Test]
        public void TestMustReadMany_Generic_Happy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<TestResource>
            {
                Status = HttpStatusCode.OK,
                Data = JsonConvert.DeserializeObject(@"{'~items':[{'Name':'Mat'},{'Name':'Tyler'}]}")
            };
            _fakeTransport.Responses.Enqueue(response);
            IList<TestResource> item = rb.MustReadMany<TestResource>();
            Assert.AreEqual("Mat", item[0]["Name"]);
            Assert.AreEqual("Tyler", item[1]["Name"]);
        }

        [Test]
        public void TestMustReadMany_Generic_Unhappy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<TestResource>
            {
                Status = HttpStatusCode.BadRequest,
                Errors = new List<Error> {new Error {Message = "Something went wrong"}}
            };
            _fakeTransport.Responses.Enqueue(response);
            Assert.Throws<ServerException>(() => rb.MustReadMany<TestResource>());
        }

        [Test]
        public void TestMustReadMany_Happy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<Dictionary<string, dynamic>>
            {
                Status = HttpStatusCode.OK,
                Data = JsonConvert.DeserializeObject(@"{'~items':[{'Name':'Mat'},{'Name':'Tyler'}]}")
            };
            _fakeTransport.Responses.Enqueue(response);
            IList<Dictionary<string, dynamic>> item = rb.MustReadMany();
            Assert.AreEqual("Mat", item[0]["Name"]);
            Assert.AreEqual("Tyler", item[1]["Name"]);
        }

        [Test]
        public void TestMustReadMany_Unhappy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<Dictionary<string, dynamic>>
            {
                Status = HttpStatusCode.BadRequest,
                Errors = new List<Error> {new Error {Message = "Something went wrong"}}
            };
            _fakeTransport.Responses.Enqueue(response);
            Assert.Throws<ServerException>(() => rb.MustReadMany());
        }

        [Test]
        public void TestMustRead_Happy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<Dictionary<string, dynamic>>
            {
                Status = HttpStatusCode.OK,
                Data = JsonConvert.DeserializeObject(@"{'Name':'Mat'}")
            };
            _fakeTransport.Responses.Enqueue(response);
            Dictionary<string, dynamic> item = rb.MustReadOne();
            Assert.AreEqual("Mat", item["Name"]);
        }

        [Test]
        public void TestMustRead_Unhappy()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<Dictionary<string, dynamic>>
            {
                Status = HttpStatusCode.BadRequest,
                Errors = new List<Error> {new Error {Message = "Something went wrong"}}
            };
            _fakeTransport.Responses.Enqueue(response);
            Assert.Throws<ServerException>(() => rb.MustReadOne());
        }

        [Test]
        public void TestNew()
        {
            var request = new RequestBuilder(_testClient, "path");
            Assert.AreEqual("path", request.Path);
            Assert.AreEqual(_testClient, request.Client);
        }

        [Test]
        public void TestParameters()
        {
            var request = new RequestBuilder(_testClient, "path");
            IDictionary<string, IList<string>> parameters = request.Parameters;
            Assert.NotNull(parameters);
            Assert.AreEqual(parameters["key"][0], _testClient.Key);
            Assert.AreEqual(request, request.With("field", "value"));
            Assert.AreEqual(parameters["field"][0], "value");
        }

        [Test]
        public void TestPathAndParams()
        {
            var request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual("people/1/books?key=abc123", request.PathAndParams());
            request.With("key1", "value1");
            request.With("key1", "value2");
            request.With("key2", "value1");
            string p = request.PathAndParams();
            Assert.True(p.StartsWith("people/1/books?"));
            Assert.True(p.Contains("key1=value1"));
            Assert.True(p.Contains("key1=value2"));
            Assert.True(p.Contains("key2=value1"));
        }


        [Test]
        public void TestRead()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            rb.Read();
            Request request = _fakeTransport.Requests.Dequeue();
            Assert.AreEqual(HttpMethod.Get.ToString(), request.HttpMethod.ToString());
            Assert.AreEqual(_testClient.UrlBase() + "people/1/books?key=abc123", request.Url);
        }

        [Test]
        public void TestReadGeneric()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var response = new DataResponse<TestResource> {Data = JsonConvert.DeserializeObject(@"{'name':'Mat'}")};
            _fakeTransport.Responses.Enqueue(response);
            DataResponse<TestResource> returnedResponse = rb.Read<TestResource>();
            TestResource item = returnedResponse.Items()[0];
            Assert.AreEqual("Mat", item.Name);
        }

        [Test]
        public void TestReplace()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var obj = new Dictionary<string, object>
            {
                {
                    "name",
                    "Tyler"
                }
            };

            _fakeTransport.Responses.Enqueue(new ChangesResponse());
            ChangesResponse response = rb.Replace(obj);
            Request request = _fakeTransport.Requests.Dequeue();
            Assert.AreEqual(HttpMethod.Put.ToString(), request.HttpMethod.ToString());
            Assert.AreEqual(_testClient.UrlBase() + "people/1/books?key=abc123", request.Url);
            Assert.AreEqual(obj, request.DataObject);
        }

        [Test]
        public void TestReplaceApplyChanges()
        {
            var mat = new TestResource {Name = "Mat"};
            var tyler = new TestResource {Name = "Tyler"};
            var ryan = new TestResource {Name = "Ryan"};

            // make a test response
            var r = new ChangesResponse
            {
                Status = HttpStatusCode.Created,
                Changes = new ChangeInfo
                {
                    Created = 1,
                    Deltas = new List<Dictionary<string, dynamic>>
                    {
                        new Dictionary<string, dynamic>
                        {
                            {"~id", "ABC123"},
                            {"something-else", "yes"}
                        }
                    }
                }
            };
            _fakeTransport.Responses.Enqueue(r);
            _testClient.At("people").Replace(mat);

            Assert.AreEqual("Mat", mat.Name);
            Assert.AreEqual("ABC123", mat.Id);
            Assert.AreEqual("yes", mat["something-else"]);
        }

        [Test]
        public void TestUpdate()
        {
            var rb = new RequestBuilder(_testClient, "people/1/books");
            var obj = new Dictionary<string, object>
            {
                {
                    "name",
                    "Tyler"
                },
                {
                    "~id",
                    "ABC123"
                }
            };

            _fakeTransport.Responses.Enqueue(new ChangesResponse());
            ChangesResponse response = rb.Update(obj);
            Request request = _fakeTransport.Requests.Dequeue();
            var patch = new HttpMethod("PATCH").ToString();
            Assert.AreEqual(patch, request.HttpMethod.ToString());
            Assert.AreEqual(_testClient.UrlBase() + "people/1/books?key=abc123", request.Url);
            Assert.AreEqual(obj, request.DataObject);
        }

        [Test]
        public void TestUpdateApplyChanges()
        {
            var mat = new TestResource {Name = "Mat"};
            var tyler = new TestResource {Name = "Tyler"};
            var ryan = new TestResource {Name = "Ryan"};

            // make a test response
            var r = new ChangesResponse
            {
                Status = HttpStatusCode.Created,
                Changes = new ChangeInfo
                {
                    Created = 1,
                    Deltas = new List<Dictionary<string, dynamic>>
                    {
                        new Dictionary<string, dynamic>
                        {
                            {"~id", "ABC123"},
                            {"something-else", "yes"}
                        }
                    }
                }
            };
            _fakeTransport.Responses.Enqueue(r);
            _testClient.At("people").Update(mat);

            Assert.AreEqual("Mat", mat.Name);
            Assert.AreEqual("ABC123", mat.Id);
            Assert.AreEqual("yes", mat["something-else"]);
        }

        [Test]
        public void TestUrl()
        {
            var request = new RequestBuilder(_testClient, "people/1/books");
            request.With("key1", "value1");
            request.With("key1", "value2");
            request.With("key2", "value1");
            string p = request.Url();
            Assert.True(p.StartsWith(_testClient.UrlBase() + "people/1/books?"));
            Assert.True(p.Contains("key1=value1"));
            Assert.True(p.Contains("key1=value2"));
            Assert.True(p.Contains("key2=value1"));
        }

        [Test]
        public void TestWhere()
        {
            var request = new RequestBuilder(_testClient, "people/1/books");
            Assert.AreEqual(request, request.Where("name", "Tyler"));
            Assert.AreEqual(request, request.Where("name", "Mat"));
            Assert.AreEqual("Tyler", request.Parameters[":name"][0]);
            Assert.AreEqual("Mat", request.Parameters[":name"][1]);
        }
    }
}