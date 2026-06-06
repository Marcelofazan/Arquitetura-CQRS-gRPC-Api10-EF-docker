using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class RelatorioDiario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? Data { get; set; }
    public double TotalFisica { get; set; }
    public double TotalJuridica { get; set; }

    [BsonElement("Clientes")]
    public List<ClienteDataModel> Clientes { get; set; }
}
