using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Reporter.Business
{
    public class ClienteMessage
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? TipoPessoa { get; set; }
        public string? CpfCnpj { get; set; }
        public string? Endereco { get; set; }
        public double Valor { get; set; }
        public string? DataCadastro { get; set; }
    }
}
