using MongoDB.Bson.Serialization.Attributes;

namespace InfraEstrutura.Producao.DataModels
{
    public record ClienteDataModel
    {
        [BsonId]
        public Guid Id { get; init; }
        public string? Nome { get; init; }
        public string? TipoPessoa { get; init; }
        public string? CpfCnpj { get; init; }
        public string? Endereco { get; init; }
        public double Valor { get; init; }
        public string? DataCadastro { get; init; }


        public static ClienteDataModel Create(Guid Id, string nome, string tipopessoa, string cpfcnpj, string endereco, double valor, string datacadastro)
        {
            return new ClienteDataModel
            {
                Id = Id,
                Nome = nome,
                TipoPessoa = tipopessoa,
                CpfCnpj = cpfcnpj,
                Endereco = endereco,
                Valor = valor,
                DataCadastro = datacadastro
            };
        }
    }
}
