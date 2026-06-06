using InfraEstrutura.Producao.Server;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using InfraEstrutura.Producao.Business.Commands;

namespace Sistema.Producao.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {

        private readonly ISender _sender;

        public ClienteController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create")]
        public async Task<ActionResult<LaunchResponse>> CriarLancamento(RegistrarCliente command)
        {
            if (command == null)
            {
                return BadRequest("O comando de lançamento não pode ser nulo.");
            }

            try
            {
                var lancamentoResponse = await _sender.Send(command);

                if (!lancamentoResponse.Success)
                {
                    return BadRequest(lancamentoResponse.Message);
                }

                return Ok(lancamentoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar o lançamento: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteLaunchResponse>> DeletarLancamento(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Id do lançamento não pode ser vazio.");
            }

            var command = new ApagarCliente(id);

            try
            {
                var result = await _sender.Send(command);

                if (!result.Success)
                {
                    return NotFound(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar o lançamento: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarLancamento(Guid id, AtualizarCliente command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID na URL não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                var result = await _sender.Send(command);

                if (result == null)
                {
                    return NotFound("Lançamento não encontrado.");
                }

                if (result.Success)
                {
                    return Ok(result.Message);
                }

                return BadRequest("Erro ao atualizar o lançamento.");
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, $"Erro ao atualizar o lançamento: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }


    }
}
