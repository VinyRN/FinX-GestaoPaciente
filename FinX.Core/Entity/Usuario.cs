using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinX.Core.Entity;

public sealed class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string SenhaSalt { get; set; } = string.Empty;
    public string Perfil { get; set; } = "usuario";
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
