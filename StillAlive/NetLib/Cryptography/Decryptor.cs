using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Virvar.Net.Cryptography
{
    public class Decryptor
    {
        private Encoding _encoding = Encoding.UTF8;
        private RSACryptoServiceProvider _rsaProvider;

        public Decryptor(Encoding encoding)
        {
            this._encoding = encoding;
            _rsaProvider = new RSACryptoServiceProvider();
        }

        public Decryptor(string xmlPrivateKey, Encoding encoding)
            : this(encoding)
        {
            _rsaProvider.FromXmlString(xmlPrivateKey);
        }

        public string GetPublicKey()
        {
            return _rsaProvider.ToXmlString(false);
        }

        public string GetPrivateKey()
        {
            return _rsaProvider.ToXmlString(true);
        }

        public string DecryptData(byte[] receiveData)
        {
            int maxLength = _rsaProvider.KeySize / 8;
            int iterCount = receiveData.Length / maxLength;
            List<byte> receiveBytes = new List<byte>(_rsaProvider.KeySize / 8 * iterCount);
            for (int i = 0; i < iterCount; i++)
            {
                byte[] tempBytes = new byte[Math.Min(receiveData.Length - i * maxLength, maxLength)];
                Buffer.BlockCopy(receiveData, i * maxLength, tempBytes, 0, tempBytes.Length);
                tempBytes = _rsaProvider.Decrypt(tempBytes, false);
                receiveBytes.AddRange(tempBytes);
            }
            return _encoding.GetString(receiveBytes.ToArray());
        }

        public byte[] Decrypt(byte[] receiveData)
        {
            int maxLength = _rsaProvider.KeySize / 8;
            int iterCount = receiveData.Length / maxLength;
            List<byte> receiveBytes = new List<byte>(_rsaProvider.KeySize / 8 * iterCount);
            for (int i = 0; i < iterCount; i++)
            {
                byte[] tempBytes = new byte[Math.Min(receiveData.Length - i * maxLength, maxLength)];
                Buffer.BlockCopy(receiveData, i * maxLength, tempBytes, 0, tempBytes.Length);
                tempBytes = _rsaProvider.Decrypt(tempBytes, false);
                receiveBytes.AddRange(tempBytes);
            }
            return receiveBytes.ToArray();
        }
    }
}
