using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Reporter.Testes
{
    public class ClienteDto
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string TipoPessoa { get; init; }
        public string CpfCnpj { get; init; }
        public string Endereco { get; init; }
        public double Valor { get; init; }
    }
}
