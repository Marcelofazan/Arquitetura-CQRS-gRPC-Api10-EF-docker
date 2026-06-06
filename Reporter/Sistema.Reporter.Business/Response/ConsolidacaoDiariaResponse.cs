using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Reporter.Business.Response
{
    public class ConsolidacaoDiariaResponse
    {
        public string? Data { get; set; }
        public double SaldoTotal { get; set; }
        public List<ClienteDto> Clientes { get; set; } = new List<ClienteDto>();

        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
    }

    public class ClienteDto
    {
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public string? TipoPessoa { get; set; }
        public string? CpfCnpj { get; set; }
        public string? Endereco { get; set; }
        public double Valor { get; set; }
        public string? DataCadastro { get; set; }
    }
}
