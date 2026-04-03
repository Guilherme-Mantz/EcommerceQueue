using Orders.Application.Orders.Commands;

namespace Orders.API.Endpoints;

public class Orders : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("", Create);
    }

    public static async Task<IResult> Create(
        ISender sender, 
        CreateOrderCommand request,
        ILogger<Orders> logger)
    {
        var result = await sender.Send(request);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Order creation failed: {Error}", result.ErrorMessage);
            return TypedResults.BadRequest(result.ErrorMessage);
        }

        return TypedResults.Created($"/orders/{result.OrderId}", new { OrderId = result.OrderId });
    }
}
