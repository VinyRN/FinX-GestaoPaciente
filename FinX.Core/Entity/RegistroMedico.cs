using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinX.Core.Entity;

public sealed class RegistroMedico
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("PatientId")]
    [BsonRepresentation(BsonType.String)]
    public Guid PacienteId { get; set; }

    [BsonElement("Date")]
    public DateTime Data { get; set; }

    [BsonElement("Type")]
    public string Tipo { get; set; } = string.Empty;

    [BsonElement("Description")]
    public string Descricao { get; set; } = string.Empty;
}
