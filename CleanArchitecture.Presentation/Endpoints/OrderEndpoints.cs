using CleanArchitecture.Presentation.Configuration;
using CleanArchitecture.Application.Entities.Orders.Queries.Get;
using CleanArchitecture.Application.Entities.Orders.Commands.Create;
using CleanArchitecture.Application.Entities.Orders.Commands.Update;
using CleanArchitecture.Application.Entities.OrderItem.Commands.Create;
using CleanArchitecture.Application.Entities.OrderItem.Commands.Delete;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/create-order", CreateOrder)
            .WithSummary("Creates a new order")
            .WithDescription("Creates a new order with the specified details.");

        app.MapPut("/update-order", UpdateOrder)
            .WithSummary("Updates an existing order")
            .WithDescription("Updates an existing order with the specified ID.");

        app.MapGet("/get-order/{id:Guid}", GetOrder)
            .WithSummary("Gets an order by ID")
            .WithDescription("Gets an order with the specified ID.");

        app.MapPost("/add-order-item", AddOrderItem)
            .WithSummary("Adds an item to an order")
            .WithDescription("Adds an item to an order with the specified details.");

        app.MapDelete("/remove-order-item", RemoveOrderItem)
            .WithSummary("Removes an item from an order")
            .WithDescription("Removes an item from an order with the specified ID.");

    }

    private static async Task<IResult> GetOrder(ISender sender, Guid id)
    {
        Result<OrderResponse> result = await sender.Send(new GetOrderQuery(id)).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> UpdateOrder(ISender sender, UpdateOrderCommand command)
    {
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> CreateOrder(ISender sender, CreateOrderCommand command)
    {
        Result<Guid> result = await sender.Send(command).ConfigureAwait(false);

        return !result.IsSuccess
            ? Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)))
            : Results.Created($"/create-order/{result.Value}", result);
    }

    private static async Task<IResult> AddOrderItem(ISender sender, CreateOrderItemCommand command)
    {
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> RemoveOrderItem(ISender sender, [FromBody] DeleteOrderItemCommand command)
    {
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }
}
