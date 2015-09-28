using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameObjects;
using GameObjects.Values;

namespace GameProcess.Actors.Client
{
    class ScoresActor : IActor, IClientPacketHandler
    {
        private GameStateClient _gameState;

        public ScoresActor(GameStateClient gameState)
        {
            this._gameState = gameState;
        }

        public void Update(GameTime gameTime)
        {
            // запрос на статистику игры по просьбе игрока
            _gameState.NetPlayer.ClientPacket.ClientQuery = Commands.ScoresButtonPressed ?
                Virvar.Net.ClientQuery.Scores :
                Virvar.Net.ClientQuery.None;
        }

        public void Receive(Virvar.Net.ServerPacket msg)
        {
            if (msg.Scores != null)
            {
                _gameState.Scores = msg.Scores;
            }
        }
    }
}
