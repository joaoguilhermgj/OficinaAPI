using Oficina.Application.DTOs;
using Oficina.Application.Service;

namespace Oficina.API.Endpoints
{
    public static class VeiculoEndpoints
    {
        public static void MapVeiculoEndpoints(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/veiculos")
                .WithTags("Veículos");

            grupo.MapGet("/", async (VeiculoService service) =>
                Results.Ok(await service.ObterTodosVeiculosAsync()));


            grupo.MapGet("/{id:int}", async (int id, VeiculoService service) =>
            {
                var veiculo = await service.ObterPorIdAsync(id);
                return veiculo is null ? Results.NotFound() : Results.Ok(veiculo);
            });

            grupo.MapPost("/", async (CriarVeiculoDto dto, VeiculoService service) =>
            {
                try
                {
                    var criado = await service.CriarAsync(dto);
                    return Results.Created($"/api/veiculos/{criado.Id}", criado);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            grupo.MapPut("/{id:int}", async (int id, AtualizarVeiculoDto dto, VeiculoService service) =>
            {
                try
                {
                    var atualizado = await service.AtualizarAsync(id, dto);
                    return atualizado ? Results.NoContent() : Results.NotFound();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            grupo.MapDelete("/{id:int}", async (int id, VeiculoService service) =>
            {
                var removido = await service.RemoverAsync(id);
                return removido ? Results.NoContent() : Results.NotFound();
            });

        }
    }
}
