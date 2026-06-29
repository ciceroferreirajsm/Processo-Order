using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Application.Orders.Queries.GetOrders;

namespace OrderManagement.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders")
            .RequireAuthorization();

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder");

        group.MapGet("/", GetOrders)
            .WithName("GetOrders");

        group.MapGet("/{id:guid}", GetOrderById)
            .WithName("GetOrderById");

        group.MapPatch("/{id:guid}/cancel", CancelOrder)
            .WithName("CancelOrder");
    }

    private static async Task<IResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.Created($"/api/orders/{result.Id}", result);
    }

    private static async Task<IResult> GetOrders(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery(
            page <= 0 ? 1 : page,
            pageSize <= 0 ? 10 : pageSize);

        var result = await mediator.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrderById(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CancelOrder(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelOrderCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
