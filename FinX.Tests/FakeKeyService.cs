using System.Security.Cryptography;
using FinX.Core.Interface.Service;

namespace FinX.Tests
{
    public class FakeKeyService : IKeyService
    {
        private readonly RSA _rsa;
        private readonly string _keyId;

        public FakeKeyService()
        {
            _rsa = RSA.Create(2048);
            _keyId = "test-key-id";
        }

        public RSA GetPrivateKey() => _rsa;
        public RSA GetPublicKey() => _rsa;
        public string GetKeyId() => _keyId;
    }

}
