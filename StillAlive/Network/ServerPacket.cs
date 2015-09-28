using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using GameObjects;
using Microsoft.Xna.Framework;
using ProtoBuf;
using GameObjects.ProtoClasses;

namespace Virvar.Net
{
    public class ServerPacket : Packet
    {
        MemoryStream stream = new MemoryStream();
        public ConcurrentDictionary<int, ProtoVector2> PlayersPositions; // метка 1
        public ConcurrentDictionary<byte, ConnectionState> PlayersConnections; // метка 2
        public ConcurrentDictionary<byte, byte> PlayersHealths; // метка 3
        public ConcurrentDictionary<byte, float> PlayersAngles; // метка 4
        public HashSet<byte> ShootingPlayers; // метка 5
        public ConcurrentDictionary<int, Score> Scores = null; // метка 6
        public ConcurrentDictionary<byte, byte> PlayersMana; // метка 7
        public ConcurrentDictionary<byte, string> PlayersNames; // метка 8

        public ServerPacket()
        {
            base.ProtocolId = 19;
            PlayersPositions = new ConcurrentDictionary<int, ProtoVector2>();
            PlayersConnections = new ConcurrentDictionary<byte, ConnectionState>();
            PlayersHealths = new ConcurrentDictionary<byte, byte>();
            PlayersAngles = new ConcurrentDictionary<byte, float>();
            ShootingPlayers = new HashSet<byte>();
            PlayersMana = new ConcurrentDictionary<byte, byte>();
            PlayersNames = new ConcurrentDictionary<byte, string>();
        }
        /// <summary>
        /// Возвращает данные для отправки.
        /// </summary>
        /// <returns></returns>
        public override byte[] Build()
        {
            long streamPos = 0;
            stream.Position = streamPos;
            byte[] baseData = base.Build();
            stream.Write(baseData, 0, baseData.Length);

            WriteConnections();
            WritePositions();
            WriteHealths();
            WriteAngles();
            WriteShooters();
            WriteScores();
            WriteMana();
            WriteNames();

            byte[] dataToSend = new byte[stream.Position - streamPos];
            stream.Position = streamPos;
            stream.Read(dataToSend, 0, dataToSend.Length);
            return dataToSend;
        }

        // записываем состояния здоровья игроков
        private void WriteHealths()
        {
            if (PlayersHealths.Count != 0)
            {
                stream.WriteByte(3);
                Serializer.SerializeWithLengthPrefix(stream, PlayersHealths, PrefixStyle.Base128);
                PlayersHealths.Clear(); // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
            }
        }
        // записываем добавленных и удалённых игроков (если они есть)
        private void WriteConnections()
        {
            if (PlayersConnections.Count != 0)
            {
                stream.WriteByte(2);
                Serializer.SerializeWithLengthPrefix(stream, PlayersConnections, PrefixStyle.Base128);
            }
        }
        // записываем в сообщение координаты игроков
        private void WritePositions()
        {
            if (PlayersPositions.Count != 0)
            {
                stream.WriteByte(1); // помечаем, что следущие данные - состояния игроков
                Serializer.SerializeWithLengthPrefix(stream, PlayersPositions, PrefixStyle.Base128);
                PlayersPositions.Clear(); // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
            }
        }
        // записываем направления взгляда игроков
        private void WriteAngles()
        {
            if (PlayersAngles.Count != 0)
            {
                stream.WriteByte(4);
                Serializer.SerializeWithLengthPrefix(stream, PlayersAngles, PrefixStyle.Base128);
                PlayersAngles.Clear(); // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
            }
        }
        // записываем стреляющих игроков
        private void WriteShooters()
        {
            lock (ShootingPlayers)
            {
                if (ShootingPlayers.Count != 0)
                {
                    stream.WriteByte(5);
                    Serializer.SerializeWithLengthPrefix(stream, ShootingPlayers, PrefixStyle.Base128);
                    ShootingPlayers.Clear(); // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
                }
            }
        }
        // записываем статистику игры
        private void WriteScores()
        {
            if (Scores != null)
            {
                stream.WriteByte(6);
                Serializer.SerializeWithLengthPrefix(stream, Scores, PrefixStyle.Base128);
            }
        }
        // записываем состояния маны игроков
        private void WriteMana()
        {
            if (PlayersMana.Count != 0)
            {
                stream.WriteByte(7);
                Serializer.SerializeWithLengthPrefix(stream, PlayersMana, PrefixStyle.Base128);
                PlayersMana.Clear(); // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
            }
        }
        // записываем имена игроков
        private void WriteNames()
        {
            if (PlayersNames.Count != 0)
            {
                stream.WriteByte(8);
                Serializer.SerializeWithLengthPrefix(stream, PlayersNames, PrefixStyle.Base128);
                PlayersNames.Clear(); // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
            }
        }

        public override long Parse(byte[] data)
        {
            long pos = base.Parse(data);
            stream = new MemoryStream(data);
            stream.Position = pos;
            int dataCode;
            while ((dataCode = stream.ReadByte()) != -1)
            {
                ParseData(dataCode);
            }
            return stream.Position;
        }

        private void ParseData(int dataLabel)
        {
            switch (dataLabel)
            {
                case 1:
                    // считываем координаты игроков
                    PlayersPositions = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<int, ProtoVector2>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 2:
                    // считываем добавленных\удалённых игроков
                    PlayersConnections = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<byte, ConnectionState>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 3:
                    // считываем состояния здоровья игроков
                    PlayersHealths = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<byte, byte>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 4:
                    // считываем направления взгляда игроков
                    PlayersAngles = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<byte, float>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 5:
                    // считываем стреляющих игроков
                    ShootingPlayers = Serializer.DeserializeWithLengthPrefix<HashSet<byte>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 6:
                    // считываем статистику игры
                    Scores = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<int, Score>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 7:
                    // считываем состояния маны игроков
                    PlayersMana = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<byte, byte>>(
                        stream, PrefixStyle.Base128);
                    break;
                case 8:
                    // считываем имекна игроков
                    PlayersNames = Serializer.DeserializeWithLengthPrefix<ConcurrentDictionary<byte, string>>(
                        stream, PrefixStyle.Base128);
                    break;
            }
        }

        public override void Clear() // ЛОГИЧЕСКАЯОШИБКА: данные могут потеряться
        {
            PlayersPositions.Clear();
            PlayersConnections.Clear();
            PlayersHealths.Clear();
            PlayersAngles.Clear();
            lock (ShootingPlayers)
            {
                ShootingPlayers.Clear();
            }
            PlayersMana.Clear();
        }
    }
    [ProtoContract]
    public enum ConnectionState
    {
        Connected,
        Disconnected
    }
}
