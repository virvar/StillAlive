using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Virvar.Net
{
    /// <summary>
    /// Нужно закрывать, когда больше не нужен
    /// </summary>
    public class TcpMessenger : IMessenger
    {
        private NetworkStream _netStream;
        private byte[] _buffer = new byte[1024];
        private List<byte> _receivedBytes = new List<byte>();

        public TcpMessenger(NetworkStream netStream)
        {
            this._netStream = netStream;
        }

        public virtual void Send(byte[] data)
        {
            _netStream.Write(data, 0, data.Length);
        }

        public virtual byte[] Receive()
        {
            _receivedBytes.Clear();
            do
            {
                int bytesCount = _netStream.Read(_buffer, 0, _buffer.Length);
                byte[] tempBytes = new byte[bytesCount];
                Buffer.BlockCopy(_buffer, 0, tempBytes, 0, bytesCount);
                _receivedBytes.AddRange(tempBytes);
            } while (_netStream.DataAvailable);
            return _receivedBytes.ToArray();
        }

        public byte[] Receive(ref System.Net.IPEndPoint remoteEP)
        {
            return Receive();
        }

        public void Close()
        {
            _netStream.Close();
        }

        ~TcpMessenger()
        {
            Close();
        }
    }
}
