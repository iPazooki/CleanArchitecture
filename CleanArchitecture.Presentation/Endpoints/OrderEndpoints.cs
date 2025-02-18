using CleanArchitecture.Application.Entities.OrderItem.Commands.Create;
using CleanArchitecture.Application.Entities.OrderItem.Commands.Delete;
using CleanArchitecture.Application.Entities.Orders.Commands.Create;
using CleanArchitecture.Application.Entities.Orders.Commands.Update;
using CleanArchitecture.Application.Entities.Orders.Queries.Get;

namespace CleanArchitecture.Presentation.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/create-order", CreateOrder)
            .WithOpenApi()
            .WithSummary("Creates a new order")
            .WithDescription("Creates a new order with the specified details.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Created, "The ID of the created order", typeof(int)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapPut("/update-order", UpdateOrder)
            .WithOpenApi()
            .WithSummary("Updates an existing order")
            .WithDescription("Updates an existing order with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NoContent, "The order was successfully updated"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapGet("/get-order/{id:int}", GetOrder)
            .WithOpenApi()
            .WithSummary("Gets an order by ID")
            .WithDescription("Gets an order with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "The order was found", typeof(OrderResponse)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, "The order was not found"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));
       
        app.MapPost("/add-order-item", AddOrderItem)
            .WithOpenApi()
            .WithSummary("Adds an item to an order")
            .WithDescription("Adds an item to an order with the specified details.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "The item was successfully added"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapDelete("/remove-order-item", RemoveOrderItem)
            .WithOpenApi()
            .WithSummary("Removes an item from an order")
            .WithDescription("Removes an item from an order with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NoContent, "The item was successfully removed"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

    }

    private static async Task<IResult> GetOrder(ISender sender, int id)
    {
        Result<OrderResponse> result = await sender.Send(new GetOrderQuery(id));

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> UpdateOrder(ISender sender, UpdateOrderCommand command)
    {
        Result result = await sender.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> CreateOrder(ISender sender, CreateOrderCommand command)
    {
        Result<int> result = await sender.Send(command);
        
        return !result.IsSuccess 
            ? Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message))) 
            : Results.Created($"/create-order/{result.Value}", result);
    }
    
    private static async Task<IResult> AddOrderItem(ISender sender, CreateOrderItemCommand command)
    {
        Result result = await sender.Send(command);

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> RemoveOrderItem(ISender sender, [FromBody] DeleteOrderItemCommand command)
    {
        Result result = await sender.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }
}