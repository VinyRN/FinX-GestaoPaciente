using System.Security.Cryptography;
using FinX.Core.Interface.Service;

namespace FinX.Service;

public sealed class CriptografiaSenha : ICriptografiaSenha
{
    private const int TamanhoSalt = 32;
    private const int TamanhoHash = 32;
    private const int Iteracoes = 210_000;

    public (string Hash, string Salt) CriarHash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Derivar(senha, salt);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool Verificar(string senha, string hash, string salt)
    {
        try
        {
            var hashEsperado = Convert.FromBase64String(hash);
            var saltBytes = Convert.FromBase64String(salt);
            var hashCalculado = Derivar(senha, saltBytes);
            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static byte[] Derivar(string senha, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
}
