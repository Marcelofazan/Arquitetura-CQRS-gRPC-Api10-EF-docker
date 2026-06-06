using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sistema.Reporter.Business.Commands;
using Sistema.Reporter.Business.Response;

namespace Sistema.Reporter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsolidacaoController : Controller
    {
        private readonly ISender _sender;

        public ConsolidacaoController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("consolidacao")]
        public async Task<ActionResult<ConsolidacaoDiariaResponse>> ObterRelatorioConsolidado([FromQuery] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest("A data é obrigatória.");
            }

            var command = new ConsolidacaoDiariaCommand(data);
            var response = await _sender.Send(command);

            if (!response.Sucesso)
            {
                return StatusCode(500, response.Mensagem);
            }

            return Ok(response);
        }
    }

}
