using System.Net;

namespace Virvar.Net
{
    /// <summary>
    /// Нужно закрывать, когда больше не нужен
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="data">отправляемые данные</param>
        void Send(byte[] data);
        /// <summary>
        /// Получение сообщения
        /// </summary>
        /// <returns></returns>
        byte[] Receive();
        /// <summary>
        /// Получение сообщения
        /// </summary>
        /// <param name="remoteEP">ip-адрес источника</param>
        /// <returns></returns>
        byte[] Receive(ref IPEndPoint remoteEP);
        /// <summary>
        /// Закрывает NetworkStream
        /// </summary>
        void Close();
    }
}
