using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProtoBuf;
using System.IO;
using System.Collections.Concurrent;
using GameObjects.ProtoClasses;

namespace Virvar.Net
{
    [Serializable]
    [ProtoContract]
    public class ClientPacket : Packet
    {
        MemoryStream stream = new MemoryStream();
        /// <summary>
        /// Id игрока.
        /// </summary>
        public byte PlayerId;
        /// <summary>
        /// Передвижения игрока. Ключ - номер пакета. (Код 1)
        /// </summary>
        public ConcurrentDictionary<ushort, Moves> Moves = new ConcurrentDictionary<ushort, Moves>();
        /// <summary>
        /// Направление взгляда персонажа (в радианах).
        /// </summary>
        public float PlayerAngle;
        /// <summary>
        /// Время стрельбы. (Код 2)
        /// </summary>
        public ConcurrentDictionary<ushort, float> Shoots = new ConcurrentDictionary<ushort, float>();
        /// <summary>
        /// Запросы клиента на необязательные данные.
        /// </summary>
        public ClientQuery ClientQuery = ClientQuery.None;
        /// <summary>
        /// Консольная команда. (Код 3)
        /// </summary>
        public ConsoleCommand ConsoleCommand;

        public ClientPacket()
        {
            base.ProtocolId = 20;
        }

        public override byte[] Build()
        {
            lock (this)
            {
                long streamPos = 0;
                stream.Position = streamPos;
                byte[] baseData = base.Build();
                stream.Write(baseData, 0, baseData.Length);
                // записывает в сообщение состояние игрока
                stream.WriteByte(PlayerId);
                // направление персонажа
                byte[] bytes = BitConverter.GetBytes(PlayerAngle);
                stream.Write(bytes, 0, bytes.Length);
                stream.WriteByte(1); // помечаем, что следущие данные - передвижения игрока
                lock (Moves)
                {
                    Serializer.SerializeWithLengthPrefix(stream, Moves, PrefixStyle.Base128);
                }
                stream.WriteByte(2); // выстрелы
                lock (Shoots)
                {
                    Serializer.SerializeWithLengthPrefix(stream, Shoots, PrefixStyle.Base128);
                }
                // запросы клиента
                stream.WriteByte((byte)ClientQuery);
                // консольная команда
                if (ConsoleCommand != null)
                {
                    stream.WriteByte(3);
                    Serializer.SerializeWithLengthPrefix(stream, ConsoleCommand, PrefixStyle.Base128);
                    ConsoleCommand = null; // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
                }
                byte[] dataToSend = new byte[stream.Position - streamPos];
                stream.Position = streamPos;
                stream.Read(dataToSend, 0, dataToSend.Length);
                return dataToSend;
            }
        }

        public override long Parse(byte[] data)
        {
            lock (this)
            {
                long pos = base.Parse(data);
                stream = new MemoryStream(data);
                stream.Position = pos;
                // считывает в сообщение состояние игрока
                this.PlayerId = (byte)stream.ReadByte();
                // направление персонажа
                byte[] bytes = new byte[4];
                stream.Read(bytes, 0, bytes.Length);
                PlayerAngle = BitConverter.ToSingle(bytes, 0);
                int dataCode = stream.ReadByte();
                if (dataCode == 1)
                {
                    // считываем передвижения игрока
                    lock (Moves)
                    {
                        Moves = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<ushort, Moves>>(stream, PrefixStyle.Base128);
                    }
                }
                dataCode = stream.ReadByte();
                if (dataCode == 2)
                {
                    // считываем выстрелы игрока
                    lock (Shoots)
                    {
                        Shoots = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<ushort, float>>(stream, PrefixStyle.Base128);
                    }
                }
                // запросы клиента
                ClientQuery = (ClientQuery)stream.ReadByte();
                // консольная команда
                dataCode = stream.ReadByte();
                if (dataCode == 3)
                {
                    ConsoleCommand = Serializer.DeserializeWithLengthPrefix<ConsoleCommand>(stream, PrefixStyle.Base128);
                }
                return stream.Position;
            }
        }

        public override void Clear()
        {
            Moves.Clear();
        }
    }

    [ProtoContract]
    public class Moves : List<Move>
    {
    }
    [ProtoContract]
    public class Move
    {
        [ProtoMember(1)]
        internal ProtoVector2 direction;
        public Vector2 Direction
        {
            get { return new Vector2(direction.X, direction.Y); }
        }
        [ProtoMember(2)]
        public TimeSpan Time;

        public Move() { }

        public Move(Vector2 direction, TimeSpan time)
        {
            this.direction = new ProtoVector2(direction.X, direction.Y);
            this.Time = time;
        }
    }
    [ProtoContract]
    [Flags]
    public enum ClientQuery : byte
    {
        None = 0,
        Scores = 1
    }
    [ProtoContract]
    public class ConsoleCommand
    {
        [ProtoMember(1)]
        public CommandCode CommandCode;
        [ProtoMember(2)]
        public string Parameters;

        public ConsoleCommand() { }

        public ConsoleCommand(CommandCode commandCode, string parameters)
        {
            this.CommandCode = commandCode;
            this.Parameters = parameters;
        }
    }
    public enum CommandCode : byte
    {
        Name = 1
    }
}
