using System.Net;

namespace Virvar.Net
{
    public interface IObjMessenger
    {
        void Send(object msg);
        void Send<T>(object msg);
        object Receive();
        object Receive<T>();
        object Receive(ref IPEndPoint remoteEP, out int packetLength);
        object Receive<T>(ref IPEndPoint remoteEP, out int packetLength);
        void Close();
    }
}
