using Microsoft.AspNetCore.Http.HttpResults;

namespace Payment.API.Endpoints
{
    public class Testes : IEndpointGroup
    {
        public static void Map(RouteGroupBuilder groupBuilder)
        {
            groupBuilder.MapGet("", Test);
        }

        public static async Task<Ok<string>> Test() 
        {
            return TypedResults.Ok("Teste de Rota");
        }
    }
}
