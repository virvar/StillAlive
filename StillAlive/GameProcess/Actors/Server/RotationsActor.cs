using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Virvar.Net;

namespace GameProcess.Actors.Server
{
    class RotationsActor : IServerPacketHandler
    {
        GameStateServer _gameState;

        public RotationsActor(GameStateServer gameState)
        {
            this._gameState = gameState;
        }

        public void Receive(ClientPacket msg)
        {
            _gameState.Players[msg.PlayerId].Angle = msg.PlayerAngle;
            foreach (var netPlayer in _gameState.NetPlayers.Values)
            {
                netPlayer.ServerPacket.PlayersAngles[msg.PlayerId] = msg.PlayerAngle;
            }
        }
    }
}
