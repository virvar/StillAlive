using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects.DrawableClasses;
using Virvar.Net;

namespace GameProcess.Actors.Client
{
    class HealthActor : IClientPacketHandler
    {
        private GameStateClient _gameState;

        public HealthActor(GameStateClient gameState)
        {
            this._gameState = gameState;
        }

        public void Receive(Virvar.Net.ServerPacket msg)
        {
            foreach (var playerHealth in msg.PlayersHealths)
            {
                // обновление состояния здоровья персонажа
                if (_gameState.Players.ContainsKey(playerHealth.Key))
                    _gameState.Players[playerHealth.Key].Health = playerHealth.Value;
            }
            foreach (var playerMana in msg.PlayersMana)
            {
                // обновление состояния маны персонажа
                if (_gameState.Players.ContainsKey(playerMana.Key))
                    _gameState.Players[playerMana.Key].Mana = playerMana.Value;
            }
        }
    }
}
