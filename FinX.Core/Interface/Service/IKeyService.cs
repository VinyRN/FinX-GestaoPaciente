using System.Security.Cryptography;

namespace FinX.Core.Interface.Service
{
    public interface IKeyService
    {
        RSA GetPrivateKey();
        RSA GetPublicKey();
        string GetKeyId();
    }
}
