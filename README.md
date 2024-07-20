
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
