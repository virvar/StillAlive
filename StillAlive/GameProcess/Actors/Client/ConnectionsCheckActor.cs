using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virvar.Net;
using System.Collections.Concurrent;
using GameObjects.ProtoClasses;

namespace GameProcess.Actors.Client
{
    class ConnectionsCheckActor : IClientPacketHandler
    {
        readonly GameStateClient _gameState;

        public ConnectionsCheckActor(GameStateClient gameState)
        {
            this._gameState = gameState;
        }

        public void Receive(Virvar.Net.ServerPacket msg)
        {
            foreach (var item in msg.PlayersConnections)
            {
                if (item.Value == Virvar.Net.ConnectionState.Connected)
                {
                    if (!_gameState.Players.ContainsKey(item.Key))
                    {
                        _gameState.CreateCharacter(item.Key);
                    }
                }
                else if (item.Value == Virvar.Net.ConnectionState.Disconnected)
                {
                    _gameState.RemoveCharacter(item.Key);
                }
            }
        }
    }
}
