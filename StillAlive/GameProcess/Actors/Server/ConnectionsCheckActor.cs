using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virvar.Net;
using System.Collections.Concurrent;
using GameObjects.ProtoClasses;

namespace GameProcess.Actors.Server
{
    class ConnectionsCheckActor : IServerPacketHandler
    {
        readonly GameStateServer _gameState;
        LinkedList<PacketChange> _changes = new LinkedList<PacketChange>();

        public ConnectionsCheckActor(GameStateServer gameState)
        {
            this._gameState = gameState;
        }
        
        public void Receive(Virvar.Net.ClientPacket msg)
        {
            // удаляем из сообщения клиенту подтверждённые данные
            List<PacketChange> changesToRemove = null;
            foreach (var change in _changes)
            {
                if (change.ServerPacketsIds.ContainsKey(msg.PlayerId) && change.ServerPacketsIds[msg.PlayerId] <= msg.Ack && msg.Ack != 0)
                {
                    ConnectionState deletedConnState;
                    // ПОТЕНЦИАЛЬНАЯОШИБКА: блокируется здесь, но не блокируется в других местах
                    lock (_gameState.NetPlayers[msg.PlayerId].ServerPacket.PlayersConnections)
                    {
                        // чтобы не было ситуаций, когда из сообщения удаляется отключения игрока, когда клиенту пришло сообщение
                        // об подключении игрока, а сообщение об отключении ещё не дошло
                        if (change.State == _gameState.NetPlayers[msg.PlayerId].ServerPacket.PlayersConnections[change.PlayerId])
                        {
                            _gameState.NetPlayers[msg.PlayerId].ServerPacket.PlayersConnections.TryRemove(change.PlayerId, out deletedConnState);
                            change.ServerPacketsIds.Remove(msg.PlayerId);
                            if (change.ServerPacketsIds.Count == 0)
                            {
                                if (changesToRemove == null)
                                    changesToRemove = new List<PacketChange>();
                                changesToRemove.Add(change);
                            }
                        }
                    }
                }
            }
            if (changesToRemove != null)
            {
                foreach (var change in changesToRemove)
                {
                    _changes.Remove(change);
                }
            }
        }

        public void AddPlayersListToPacket(NetPlayer netPlayer)
        {
            // добавляем в пакет нового игрока информацию о состоянии игры
            foreach (var character in _gameState.Players.Values)
            {
                if (character.Id != netPlayer.PlayerId)
                {
                    // подключения
                    netPlayer.ServerPacket.PlayersConnections.TryAdd(
                        (byte)character.Id, Virvar.Net.ConnectionState.Connected);
                    PacketChange curChange1 = new PacketChange((byte)character.Id, ConnectionState.Connected);
                    curChange1.ServerPacketsIds.Add(netPlayer.PlayerId, netPlayer.ServerPacket.Sequence);
                    _changes.AddLast(curChange1);
                    // позиции
                    netPlayer.ServerPacket.PlayersPositions.TryAdd(
                        (byte)character.Id, (ProtoVector2)character.Position
                    );
                    // уровни здоровья
                    netPlayer.ServerPacket.PlayersHealths.TryAdd(
                        (byte)character.Id, (byte)character.Health);
                    // имена
                    netPlayer.ServerPacket.PlayersNames.TryAdd(
                        (byte)character.Id, character.Name);
                }
            }
            foreach (var character in _gameState.Bots)
            {
                netPlayer.ServerPacket.PlayersConnections.TryAdd(
                    (byte)character.Id, Virvar.Net.ConnectionState.Connected);
                PacketChange curChange1 = new PacketChange((byte)character.Id, ConnectionState.Connected);
                curChange1.ServerPacketsIds.Add(netPlayer.PlayerId, netPlayer.ServerPacket.Sequence);
                _changes.AddLast(curChange1);
                netPlayer.ServerPacket.PlayersPositions.TryAdd(
                    (byte)character.Id, (ProtoVector2)character.Position
                );
                netPlayer.ServerPacket.PlayersHealths.TryAdd(
                    (byte)character.Id, (byte)character.Health);
            }
            // добавляем в пакет каждого игрока информацию о новом игроке
            PacketChange curChange2 = new PacketChange((byte)netPlayer.PlayerId, ConnectionState.Connected);
            foreach (var nPlayer in _gameState.NetPlayers.Values)
            {
                if (nPlayer.PlayerId != netPlayer.PlayerId)
                {
                    nPlayer.ServerPacket.PlayersConnections.TryAdd(
                        netPlayer.PlayerId, Virvar.Net.ConnectionState.Connected);
                    curChange2.ServerPacketsIds.Add(nPlayer.PlayerId, nPlayer.ServerPacket.Sequence);
                    nPlayer.ServerPacket.PlayersPositions.TryAdd(
                        netPlayer.PlayerId, (ProtoVector2)netPlayer.Character.Position
                    );
                    nPlayer.ServerPacket.PlayersHealths.TryAdd(
                        netPlayer.PlayerId, (byte)netPlayer.Character.Health);
                }
            }
            if (curChange2.ServerPacketsIds.Count > 0)
                _changes.AddLast(curChange2);
        }

        public void SetPlayerDisconnected(byte playerId)
        {
            PacketChange curChange = new PacketChange(playerId, ConnectionState.Disconnected);
            foreach (var netPlayer in _gameState.NetPlayers.Values)
            {
                curChange.ServerPacketsIds.Add(netPlayer.PlayerId, netPlayer.ServerPacket.Sequence);
                netPlayer.ServerPacket.PlayersConnections[playerId] = ConnectionState.Disconnected;
            }
            _changes.AddLast(curChange);
        }

        class PacketChange
        {
            private static uint _currentId = 1;
            public uint Id;
            public byte PlayerId;
            public ConnectionState State;
            public Dictionary<byte, ushort> ServerPacketsIds = new Dictionary<byte, ushort>();

            public PacketChange(byte playerId, ConnectionState state)
            {
                this.Id = _currentId++;
                this.PlayerId = playerId;
                this.State = state;
            }
        }
    }
}
