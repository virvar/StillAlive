using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ProtoBuf.Serializers;
using System.IO;
using System.Net;
using ProtoBuf.Meta;

namespace Virvar.Net
{
    [ProtoContract]
    public class ProtoMessenger<T> : IObjMessenger, IDisposable
    {
        private IMessenger _messenger;
        private Stream _stream;

        public ProtoMessenger(IMessenger messenger)
        {
            this._messenger = messenger;
            _stream = new MemoryStream();
        }

        public void Send(object msg)
        {
            Send<T>(msg);
        }

        public void Send<T1>(object msg)
        {
            long streamPos = _stream.Position;
            Serializer.Serialize<T1>(_stream, (T1)msg);
            _stream.Position = streamPos;
            byte[] bytes = new byte[_stream.Length - streamPos];
            int count = _stream.Read(bytes, 0, bytes.Length);
            _messenger.Send(bytes);
        }

        public object Receive()
        {
            return Receive<T>();
        }

        public object Receive<T1>()
        {
            IPEndPoint remoteEP = null;
            int packetLength;
            return Receive<T1>(ref remoteEP, out packetLength);
        }

        public object Receive(ref IPEndPoint remoteEP, out int packetLength)
        {
            return Receive<T>(ref remoteEP, out packetLength);
        }

        public object Receive<T1>(ref IPEndPoint remoteEP, out int packetLength)
        {
            // получение данных
            byte[] bytes = _messenger.Receive(ref remoteEP);
            if (bytes == null)
            {
                packetLength = 0;
                return null;
            }
            packetLength = bytes.Length;
            long streamPos = _stream.Position;
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Position = streamPos;
            object msg = Serializer.Deserialize<T1>(_stream);
            return msg;
        }

        public void Close()
        {
            _messenger.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
