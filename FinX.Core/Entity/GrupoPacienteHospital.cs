namespace FinX.Core.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class GrupoPacienteHospital
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public Guid HospitalId { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }

}
