using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Virvar.Net;

namespace GameProcess.Actors.Client
{
    class RotationsActor : IActor, IClientPacketHandler
    {
        private GameStateClient _gameState;

        public RotationsActor(GameStateClient gameState)
        {
            this._gameState = gameState;
        }

        public void Update(GameTime gameTime)
        {
            _gameState.NetPlayer.ClientPacket.PlayerAngle = _gameState.Player.Angle;
        }

        public void Receive(ServerPacket msg)
        {
            foreach (var item in msg.PlayersAngles)
            {
                _gameState.Players[item.Key].Angle = item.Value;
            }
        }
    }
}
