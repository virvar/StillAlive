using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.IO;

namespace Virvar.Net
{
    public abstract class Packet
    {
        protected byte ProtocolId;
        /// <summary>
        /// Порядковый номер пакета
        /// </summary>
        public ushort Sequence = 1;
        /// <summary>
        /// Порядковый номер самого свежего полученного пакета
        /// </summary>
        public ushort Ack;
        /// <summary>
        /// Состояния последних 32 пакетов перед самым свежим (полчено\не получено).
        /// Самый левый бит - самый старый пакет, самый правый - предпоследний пакет
        /// </summary>
        public uint AckBits;
        /// <summary>
        /// Время отправки сообщения. Не передаётся.
        /// </summary>
        public TimeSpan Time;
        /// <summary>
        /// Возвращает данные для отправки.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Build()
        {
            List<byte> dataToSend = new List<byte>(5);
            dataToSend.Add(ProtocolId);
            dataToSend.AddRange(BitConverter.GetBytes(Sequence));
            dataToSend.AddRange(BitConverter.GetBytes(Ack));
            return dataToSend.ToArray();
        }
        /// <summary>
        /// Приводит состояние объекта в соотвествие с полученными данными.
        /// Возвращает количество прочитанных байтов.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual long Parse(byte[] data)
        {
            if (data[0] != ProtocolId)
                throw new Exception("Неправильный протокол сообщения.");
            this.Sequence = BitConverter.ToUInt16(data, 1);
            this.Ack = BitConverter.ToUInt16(data, 3);
            return 5;
        }

        public abstract void Clear();
    }
}
