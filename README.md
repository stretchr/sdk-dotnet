# Stretchr .NET SDK - BETA

The Stretchr .NET allows you to interact with the Stretchr data services using native .NET code.

The Stretchr .NET SDK is a Portable Class Library that supports:

  * .NET Framework 4.5
  * Silverlight 5
  * Windows 8
  * Windows Phone Silverlight 8

## Cheatsheet

The Stretchr .NET SDK provides a powerful set of capabilities that allow you to interact with the remote data services.

### Client

Before you can do anything with the Stretchr .NET SDK, you need to create a client.  We recommend that you store the client in an accessible location, and usually you will only need one per application.

The `Client` holds your Stretchr account, project and API key details.

```cs
// declare some property to hold the client...
public Client Stretchr { get; set; }

// then at the start of your program, create the client...
Stretchr = new Client("account", "project", "api-key");
```

  * You will *always* need a Stretchr.Client, so you might as well create one when your application starts
  * Naming your client `Stretchr` (rather than say, `Client`) makes code that uses it later very obvious

### Building requests

Once you have your client, you use the `At` method to start a request.  The `At` method takes a path, which is a relative path (i.e. don't worry about `http` and all that), that forms the basis of the request.

You use modifier methods (such as `Order`, `Limit`, `Skip`, `WithTotal` etc.) to add parameters to your request before calling an action method, which will cause the request to actually be made.

### Resources

Stretchr stores data inside resources, your code will need some to represent these resources somehow.

#### Typeless resources

While the Stretchr .NET SDK uses loose types (like `Dictionary` and `List`) where possible, it provides a handy `Resource` type that gives you some additional functionality.

To create a resource using the `Resource` class:

```cs
var person = new Resource {{"name", "Mat"}, {"age", 30}};
```

  * Since `Resource` inherits from `Dictionary`, you can make use of the dictionary capabilities

#### Subclassing Resource to build your own types

A common approach is to strongly type resources that you wish to work with.  Stretchr supports schemaless resources, so this is only a convenient design option, but is a good way to get started if you aren't working with existing data.

```cs
public class Person : Resource
{
    public string Name
    {
        get { return Get("name"); }
        set { Set("name", value); }
    }

    public int Age
    {
        get { return Get("age"); }
        set { Set("age", value); }
    }
}
```

  * Inherit the `Resource` class
  * Use `Get` and `Set` methods inside property accessors

### Fluent design

The Stretchr .NET SDK provides a fluent design where each modifier method returns `this`, allowing you to chain them into sentences:

```
var response = Stretchr.At("people").Order("name").Limit(10).WithTotal().Read();
```

  * Most modifier methods return `this` to allow chaining
  * The `Read` method is an action method, and that returns a `Response` of some kind

### Actions

Once you have build your request using the modifiers (starting with the `At` method), you can call an action method which will actually perform a request against the remote server.

#### Reading many resources

There are a few ways you can read resources, but the most common is the `Read` method.  It simply makes the request and returns the response.

```cs
var response = Stretchr.At("people").Read();
if (response.IsSuccess)
{
	foreach (var item in repsonse.Items()) 
	{
		// TODO: do something with the item
	}
}
```

If you have strongly typed your resources by subclassing `Resource`, you can ask for that specific type back:

```cs
var response = Stretchr.At("people").Read<Person>();
```

The resulting `Items` in the response will be `Person` objects.

#### Reading one resource

If you want to read a specific resource, you can still use the `Read` method like above, except the path will not be to a collection, and will instead include an ID:

```cs
var response = Stretchr.At("people/123").Read();
if (response.IsSuccess)
{
	var person123 = response.Items().First();
}
```

Alternatively, if you're confident that the resource exists (and its an error for it not to) you can use the `Must` versions, which simplifies the interface:

```cs
var person123 = Stretchr.At("people/123").MustReadOne();
```

  * While the `Must*` methods have a simpler interface, they will throw exceptions if there is anything wrong with the requests, rather than returning the response and letting you check for `IsSuccess`.

#### Creating resources

To save data in Stretchr, simply create your resource somehow and use the `Create` method:

```cs
var response = Stretchr.At("people").Create(resource);
```

  * Look in the `ExampleProject` code for examples of different ways to do this

If the server generates any information in response to an action, and provided your resource implements the `IUpdatable` interface, the Stretchr .NET SDK will update your resource with that information.  Examples of this in action include IDs, timestamps, etc.

You can also create many resources at the same time just by passing an `IList` of resources into the `Create` method.

### Updating and replacing resources

You can use the `Update` and `Replace` methods to perform those actions against existing resources.  For more information, consult the [Stretchr API documentation](http://docs.stretchr.com/).