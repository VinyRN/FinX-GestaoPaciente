namespace FinX.Core.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class Paciente
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonElement("Name")]
        public string Nome { get; set; } = string.Empty;
        [BsonElement("CPF")]
        public string Cpf { get; set; } = string.Empty;
        [BsonElement("DateOfBirth")]
        public DateTime DataNascimento { get; set; }
        [BsonElement("Contact")]
        public string Contato { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }

}
