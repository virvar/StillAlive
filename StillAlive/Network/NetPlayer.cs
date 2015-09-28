using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using Virvar.Net;

namespace Virvar.Net
{
    /// <summary>
    /// Класс, отвечающий за передачу данных.
    /// </summary>
    public class NetPlayer
    {
        public byte PlayerId { get { return (byte)Character.Id; } }
        public IMessenger Messenger;
        public Character Character;
        /// <summary>
        /// Когда был получен последний пакет от игрока.
        /// </summary>
        public TimeSpan LastUpdate;
        public readonly ServerPacket ServerPacket = new ServerPacket(); // пакет, отправляемый клиенту
        public readonly ClientPacket ClientPacket = new ClientPacket(); // пакет, отправляемый клиентом

        public NetPlayer(IMessenger messenger, Character character)
        {
            this.Messenger = messenger;
            this.Character = character;
            ClientPacket.PlayerId = (byte)character.Id;
        }

        public NetPlayer(IMessenger messenger, Character character, TimeSpan createTime)
            : this(messenger, character)
        {
            LastUpdate = createTime;
        }

        public void SendToServer(byte[] data)
        {
            //Console.WriteLine("Packet length: " + data.Length);
            Messenger.Send(data);
            ClientPacket.Sequence++;
        }

        public void SendToClient(byte[] data)
        {
            Messenger.Send(data);
            ServerPacket.Sequence++;
        }

        public void Close()
        {
            Messenger.Close();
        }
    }
}
