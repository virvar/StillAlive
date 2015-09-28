using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virvar.Net;
using Microsoft.Xna.Framework;
using GameObjects.DrawableClasses;
using GameObjects;
using GameObjects.ProtoClasses;

namespace GameProcess.Actors.Client
{
    class HitsActor : IActor, IClientPacketHandler
    {
        private GameStateClient _gameState;

        public HitsActor(GameStateClient gameState)
        {
            this._gameState = gameState;
        }

        public void Update(GameTime gameTime)
        {
            ushort sequence = _gameState.NetPlayer.ClientPacket.Sequence;
            lock (_gameState.NetPlayer.ClientPacket.Shoots)
            {
                lock (_gameState.Player)
                {
                    _gameState.NetPlayer.ClientPacket.Shoots[sequence] = _gameState.Player.ShootingTime;
                    _gameState.Player.ShootingTime = 0;
                }
            }
        }

        public void Receive(ServerPacket msg)
        {
            // анимация выстрела
            foreach (var player in _gameState.Players.Values)
            {
                player.IsShooting = false;
            }
            lock (msg.ShootingPlayers)
            {
                foreach (var player in msg.ShootingPlayers)
                {
                    _gameState.Players[player].IsShooting = true;
                }
            }
            // удаляем из пакета выстрелы игрока, которые уже были обработаны сервером
            HashSet<ushort> keysToRemove = new HashSet<ushort>();
            foreach (var shootSeq in _gameState.NetPlayer.ClientPacket.Shoots.Keys)
            {
                if (msg.Ack > shootSeq)
                {
                    keysToRemove.Add(shootSeq);
                }
            }
            float deletedValue;
            foreach (var key in keysToRemove)
            {
                _gameState.NetPlayer.ClientPacket.Shoots.TryRemove(key, out deletedValue);
            }
        }
    }
}
