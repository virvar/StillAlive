using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Virvar.Net;

namespace GameProcess.Actors.Server
{
    class ConsoleCommandsActor : IServerPacketHandler
    {
        GameStateServer _gameState;

        public ConsoleCommandsActor(GameStateServer gameState)
        {
            this._gameState = gameState;
        }

        public void Receive(ClientPacket msg)
        {
            if (msg.ConsoleCommand != null)
            {
                switch (msg.ConsoleCommand.CommandCode)
                {
                    case CommandCode.Name:
                        ChangeName(msg.PlayerId, msg.ConsoleCommand.Parameters);
                        break;
                }
                msg.ConsoleCommand = null;
            }
        }

        private void ChangeName(int playerId, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _gameState.Players[playerId].Name = name;
                foreach (var netPlayer in _gameState.NetPlayers.Values)
                {
                    netPlayer.ServerPacket.PlayersNames[(byte)playerId] = name;
                }
            }
        }
    }
}
