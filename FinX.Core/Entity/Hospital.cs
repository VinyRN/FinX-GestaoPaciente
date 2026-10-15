namespace FinX.Core.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class Hospital
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
    }

}
