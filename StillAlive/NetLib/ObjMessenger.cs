using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virvar.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace Virvar.Net
{
    public class ObjMessenger
    {
        private IMessenger _messenger;
        private Stream _stream;
        private BinaryFormatter _formatter;

        public ObjMessenger(IMessenger messenger)
        {
            this._messenger = messenger;
            _stream = new MemoryStream();
            _formatter = new BinaryFormatter();
        }

        public void Send(object msg)
        {
            long streamPos = _stream.Position;
            _formatter.Serialize(_stream, msg);
            _stream.Position = streamPos;
            byte[] bytes = new byte[_stream.Length - streamPos];
            int count = _stream.Read(bytes, 0, bytes.Length);
            _messenger.Send(bytes);
        }

        public object Receive()
        {
            IPEndPoint remoteEP = null;
            int packetLength;
            return Receive(ref remoteEP, out packetLength);
        }

        public object Receive(ref IPEndPoint remoteEP, out int packetLength)
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
            object msg = _formatter.Deserialize(_stream);
            return msg;
        }

        public void Close()
        {
            _messenger.Close();
        }
    }
}
