namespace FinX.Core.Interface.Service;

public interface ICriptografiaSenha
{
    (string Hash, string Salt) CriarHash(string senha);
    bool Verificar(string senha, string hash, string salt);
}
