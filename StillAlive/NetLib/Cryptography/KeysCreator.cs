using System.Security.Cryptography;

namespace Virvar.Net.Cryptography
{
    public class KeysCreator
    {
        public static void CreateKeys(out string publicKey, out string privateKey)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            publicKey = rsaProvider.ToXmlString(false);
            privateKey = rsaProvider.ToXmlString(true);
        }
    }
}
