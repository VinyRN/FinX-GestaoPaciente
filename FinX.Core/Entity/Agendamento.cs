namespace FinX.Core.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class Agendamento
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid PacienteId { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public string Status { get; set; } = "Agendado";
    }

}
