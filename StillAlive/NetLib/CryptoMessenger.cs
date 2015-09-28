using System.Net.Sockets;
using Virvar.Net.Cryptography;

namespace Virvar.Net
{
    public class CryptoMessenger : TcpMessenger
    {
        public Decryptor Decryptor { get; set; }
        public Encryptor Encryptor { get; set; }

        public CryptoMessenger(NetworkStream netStream)
            : base(netStream) { }

        public CryptoMessenger(NetworkStream netStream, Decryptor decryptor, Encryptor encryptor)
            : base(netStream)
        {
            this.Decryptor = decryptor;
            this.Encryptor = encryptor;
        }

        public override void Send(byte[] data)
        {
            byte[] msgBytes = Encryptor.EncryptData(data);
            base.Send(msgBytes);
        }

        public override byte[] Receive()
        {
            byte[] data = base.Receive();
            return Decryptor.Decrypt(data);
        }
    }
}
