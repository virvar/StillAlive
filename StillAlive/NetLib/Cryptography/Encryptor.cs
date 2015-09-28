using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Virvar.Net.Cryptography
{
    public class Encryptor
    {
        private Encoding _encoding = Encoding.UTF8;
        private RSACryptoServiceProvider _rsaProvider;

        public Encryptor(string xmlPublicKey, Encoding encoding)
        {
            this._encoding = encoding;
            _rsaProvider = new RSACryptoServiceProvider();
            _rsaProvider.FromXmlString(xmlPublicKey);
        }

        public byte[] EncryptData(string dataToSend)
        {
            byte[] clearText = _encoding.GetBytes(dataToSend);
            int maxLength = _rsaProvider.KeySize / 8 - 11;
            int iterCount = clearText.Length / maxLength;
            List<byte> bytesToSend = new List<byte>(_rsaProvider.KeySize / 8 * iterCount);
            for (int i = 0; i <= iterCount; i++)
            {
                byte[] tempBytes = new byte[Math.Min(clearText.Length - i * maxLength, maxLength)];
                Buffer.BlockCopy(clearText, i * maxLength, tempBytes, 0, tempBytes.Length);
                bytesToSend.AddRange(_rsaProvider.Encrypt(tempBytes, false));
            }
            return bytesToSend.ToArray();
        }

        public byte[] EncryptData(byte[] dataToSend)
        {
            int maxLength = _rsaProvider.KeySize / 8 - 11;
            int iterCount = dataToSend.Length / maxLength;
            List<byte> bytesToSend = new List<byte>(_rsaProvider.KeySize / 8 * iterCount);
            for (int i = 0; i <= iterCount; i++)
            {
                byte[] tempBytes = new byte[Math.Min(dataToSend.Length - i * maxLength, maxLength)];
                Buffer.BlockCopy(dataToSend, i * maxLength, tempBytes, 0, tempBytes.Length);
                bytesToSend.AddRange(_rsaProvider.Encrypt(tempBytes, false));
            }
            return bytesToSend.ToArray();
        }
    }
}
