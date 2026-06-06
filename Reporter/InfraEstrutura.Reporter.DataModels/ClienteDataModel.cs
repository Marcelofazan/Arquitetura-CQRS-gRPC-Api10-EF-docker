using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class ClienteDataModel
{
    [BsonElement("Id")]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }
    public string? Nome { get; init; }
    public string? TipoPessoa { get; init; }
    public string? CpfCnpj { get; init; }
    public string? Endereco { get; init; }
    public double Valor { get; init; }
    public string? DataCadastro { get; init; }
}
