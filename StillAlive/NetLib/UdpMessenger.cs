using System;
using System.Net;
using System.Net.Sockets;

namespace Virvar.Net
{
    public class UdpMessenger : IMessenger, IDisposable
    {
        private UdpClient _updClient;
        private string _host;
        private int _remotePort;

        public UdpMessenger(int localPort)
        {
            _updClient = new UdpClient(localPort);
        }

        public UdpMessenger(int localPort, int receiveTimeout)
            : this(localPort)
        {
            _updClient.Client.ReceiveTimeout = receiveTimeout;
        }

        public UdpMessenger(string host, int remotePort)
        {
            this._host = host;
            this._remotePort = remotePort;
            _updClient = new UdpClient();
        }

        public UdpMessenger(string host, int remotePort, int localPort)
            : this(localPort)
        {
            this._host = host;
            this._remotePort = remotePort;
        }

        public UdpMessenger(string host, int remotePort, int localPort, int receiveTimeout)
            : this(host, remotePort, localPort)
        {
            _updClient.Client.ReceiveTimeout = receiveTimeout;
        }

        /// <summary>
        /// Отправить данные
        /// </summary>
        /// <param name="datagram">данные</param>
        public void Send(byte[] datagram)
        {
            try
            {
                _updClient.Send(datagram, datagram.Length, _host, _remotePort);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());
                Close();
            }
        }

        /// <summary>
        /// Получить данные
        /// </summary>
        /// <returns></returns>
        public byte[] Receive()
        {
            IPEndPoint ipEndPoint = null;
            return Receive(ref ipEndPoint);
        }

        /// <summary>
        /// Получить данные
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            byte[] data = null;
            try
            {
                data = _updClient.Receive(ref remoteEP);
            }
            catch
            {
                //Close();
            }
            return data;
        }

        public void Close()
        {
            if (_updClient != null)
                _updClient.Close();
            if (_updClient != null)
                _updClient.Close();
        }

        public void Dispose()
        {
            Close();
        }

        ~UdpMessenger()
        {
            Close();
        }
    }
}
