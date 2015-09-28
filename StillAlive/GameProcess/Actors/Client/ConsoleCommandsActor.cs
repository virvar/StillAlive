using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Virvar.Net;

namespace GameProcess.Actors.Client
{
    class ConsoleCommandsActor : IClientPacketHandler
    {
        private GameStateClient _gameState;

        public ConsoleCommandsActor(GameStateClient gameState)
        {
            this._gameState = gameState;
        }

        public void Receive(ServerPacket msg)
        {
            // применяем новые имена
            foreach (var playerName in msg.PlayersNames)
            {
                _gameState.Players[playerName.Key].Name = playerName.Value;
            }
        }
    }
}
