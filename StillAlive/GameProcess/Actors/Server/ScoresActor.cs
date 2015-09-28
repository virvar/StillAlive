using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameObjects;
using GameObjects.Values;

namespace GameProcess.Actors.Server
{
    class ScoresActor : IServerPacketHandler
    {
        GameStateServer _gameState;

        public ScoresActor(GameStateServer gameState)
        {
            this._gameState = gameState;
        }

        public void Receive(Virvar.Net.ClientPacket msg)
        {
            // отправить клиенту статистику игры, если нужно
            _gameState.NetPlayers[msg.PlayerId].ServerPacket.Scores = (msg.ClientQuery == Virvar.Net.ClientQuery.Scores) ?
                _gameState.Scores :
                null;
        }
    }
}
