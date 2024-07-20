
# Services

## AddEndpointsApiExplorer

Purpose: This service registers an API explorer that helps generate metadata for your API endpoints.
Usage: It is used to automatically generate API documentation and description information for minimal APIs in ASP.NET Core.
When to Use: This is useful when you want to expose the details of your API endpoints for documentation purposes, especially for OpenAPI/Swagger documentation.

## AddSwaggerGen

Purpose: This service registers the Swagger generator that will produce the Swagger/OpenAPI documentation for your API.
Usage: It is used to generate the Swagger JSON and Swagger UI for your API, which provides a visual representation of your API’s endpoints, request/response types, and other relevant information.
When to Use: This is used when you want to enable Swagger/OpenAPI support in your application, making it easier for developers to understand, interact with, and test your API

Traditional Controllers: You can use AddSwaggerGen alone.
Minimal APIs: It's recommended to use both AddEndpointsApiExplorer and AddSwaggerGen to ensure comprehensive metadata generation and documentation.

# Response

## return OkObjectResult

### return CouponStore.couponList;
Implicit Return: When you return CouponStore.couponList directly, the framework implicitly decides how to serialize this data and what HTTP status code to use. By default, it will serialize the list to JSON and return a 200 OK status.

### Results.Ok(CouponStore.couponList);
Explicit Return: This uses the Results class to explicitly create an OkObjectResult, which means you are specifically stating that you want to return the CouponStore.couponList with a 200 OK status.

Benefits of Using Results.Ok

Explicitness: Makes it clear what HTTP status code is being returned.
Consistency: Ensures that if other status codes need to be returned (e.g., BadRequest, NotFound), the same pattern can be followed.
Readability: Improves code readability by making the intention explicit.

### Results.CreatedAtRoute vs Results.Created

Why Use CreatedAtRoute?
Route Naming:
CreatedAtRoute allows you to reference a named route, making the code more robust and maintainable.
If the route changes, you only need to update the route definition, not every occurrence where the route is used.

Consistency:
Ensures that the URI returned is always consistent with the defined route.
Reduces the risk of hardcoding errors in the URI.

Flexibility:
Using named routes provides flexibility in URL structure. If the route template changes, you update it in one place (the route definition) rather than in every location where the URI is used.
Self-Documentation:
By using CreatedAtRoute, you are providing clients with a clear and explicit way to access the newly created resource via a named route, which can improve the clarity of your API documentation.

### Summary
Use CreatedAtRoute for a more maintainable and flexible approach, leveraging named routes to ensure consistency and reduce hardcoding.
Use Created when you prefer a simpler approach and the URI structure is unlikely to change.


# Request

## Route Parameter

### app.MapGet("/api/coupon{id:int}", () => { /* implementation */ });
Parameter Handling: The route parameter id is not directly accessible in the handler because it is not declared as a parameter in the lambda function. You would need to retrieve it from the route data manually within the function body if needed.

### app.MapGet("/api/coupon{id:int}", (int id) => { /* implementation */ });
Parameter Handling: The route parameter id is directly passed as a parameter to the lambda function, making it immediately accessible and strongly typed.

## Summary
While both approaches work, using the route parameter directly in the lambda function is preferred due to:
Simplicity: Directly using the parameter in the lambda function is simpler and more readable.
Type Safety: The parameter is strongly typed, reducing the risk of type conversion errors.
Clean Code: Avoids additional code to extract the parameter from the route data manually.


## Blocking Asynchronous Call
`
app.MapPost("/api/coupon",
         (IMapper _mapper,
         IValidator<CouponCreateDTO> _validation,
         [FromBody] CouponCreateDTO coupon_createDTO) => 
{
   var validationResult = _validation.ValidateAsync(coupon_createDTO).GetAwaiter().GetResult();
   // Further processing...
});
`
Synchronous Execution: Although the method being called is asynchronous, GetAwaiter().GetResult() blocks the current thread until the asynchronous operation is complete.
Thread Blocking: This can lead to thread pool starvation if used frequently in a high-concurrency environment, as it blocks a thread that could otherwise be used to process other requests.
Deadlocks: It can cause deadlocks in some synchronization contexts, especially in UI applications or when there are certain types of synchronization contexts present (e.g., ASP.NET Classic).
Simpler Code for Sync Contexts: It might be simpler in scenarios where you need to convert async code to sync, but it’s generally not recommended.

## Fully Asynchronous Call
`
app.MapPost("/api/coupon",
         async (IMapper _mapper,
         IValidator<CouponCreateDTO> _validation,
         [FromBody] CouponCreateDTO coupon_createDTO) => 
{
   var validationResult = await _validation.ValidateAsync(coupon_createDTO);
   // Further processing...
});
`
Asynchronous Execution: This approach allows the method to be fully asynchronous, which is non-blocking and yields the thread back to the thread pool while waiting for the asynchronous operation to complete.
Better Performance and Scalability: It improves performance and scalability by not blocking threads, allowing the application to handle more concurrent requests efficiently.
Avoids Deadlocks: This approach avoids the risk of deadlocks associated with blocking asynchronous calls.
Modern Best Practice: Using await for asynchronous calls is the modern best practice in .NET applications.

## When Blocking Asynchronous Calls is unavoidable

Blocking asynchronous calls with `GetAwaiter().GetResult()` or similar methods (like `.Result` or `.Wait()`) is generally discouraged due to the potential issues it can cause, such as deadlocks and thread pool starvation. However, there are some scenarios where it might be necessary or where developers might find themselves needing to use it. Here are a few situations where blocking asynchronous calls might be considered necessary:

### 1. Synchronous APIs

#### Context:
When working with APIs that do not support asynchronous operations and you need to call an asynchronous method.

#### Example:
You have a synchronous method that is part of an interface or a base class that you cannot change to async.

```csharp
public class SyncService
{
    private readonly IAsyncService _asyncService;

    public SyncService(IAsyncService asyncService)
    {
        _asyncService = asyncService;
    }

    public void PerformSyncOperation()
    {
        var result = _asyncService.PerformAsyncOperation().GetAwaiter().GetResult();
        // Continue with synchronous logic
    }
}
```

### 2. Application Startup Code

#### Context:
During application startup (e.g., in `Main` or `ConfigureServices` methods), where asynchronous code needs to run but the startup process itself is synchronous.

#### Example:
Initial configuration or setup that must complete before the application can start accepting requests.

```csharp
public static void Main(string[] args)
{
    var host = CreateHostBuilder(args).Build();

    // Synchronously initialize async tasks
    var asyncInitTask = host.Services.GetRequiredService<IAsyncInitializer>().InitializeAsync();
    asyncInitTask.GetAwaiter().GetResult();

    host.Run();
}
```

### 3. Unit Testing

#### Context:
In some unit testing frameworks or scenarios, you might need to synchronously wait for an asynchronous operation to complete.

#### Example:
A test setup or teardown method that requires waiting for an asynchronous operation.

```csharp
[SetUp]
public void Setup()
{
    var task = _asyncService.InitializeAsync();
    task.GetAwaiter().GetResult();
}

[Test]
public void TestMethod()
{
    var result = _asyncService.SomeAsyncMethod().GetAwaiter().GetResult();
    Assert.AreEqual(expected, result);
}
```

### 4. Legacy Code Integration

#### Context:
When integrating with legacy systems or libraries that do not support async/await, and you need to call asynchronous code from synchronous code.

#### Example:
Calling new asynchronous APIs from legacy synchronous code paths.

```csharp
public class LegacySystemAdapter
{
    private readonly INewAsyncService _newAsyncService;

    public LegacySystemAdapter(INewAsyncService newAsyncService)
    {
        _newAsyncService = newAsyncService;
    }

    public void CallLegacyCode()
    {
        var result = _newAsyncService.PerformOperationAsync().GetAwaiter().GetResult();
        // Use result in legacy code
    }
}
```

### Considerations and Risks

- **Deadlocks:** Blocking calls can lead to deadlocks, especially in environments with synchronization contexts, like ASP.NET or UI applications.
- **Thread Pool Starvation:** Blocking a thread that is waiting for an async operation can exhaust the thread pool under high load.
- **Maintainability:** It can make code harder to maintain and reason about, as it mixes async and sync paradigms.

### Best Practices

- **Minimize Use:** Use blocking calls sparingly and only when absolutely necessary.
- **Async All the Way:** Prefer using async/await all the way down the call stack to avoid the need for blocking calls.
- **Isolate Blocking Calls:** If you must use blocking calls, isolate them in a way that minimizes their impact on the rest of the application.
