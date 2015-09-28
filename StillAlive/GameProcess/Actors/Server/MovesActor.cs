using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects.DrawableClasses;
using Microsoft.Xna.Framework;
using Virvar.Net;
using GameObjects.ProtoClasses;

namespace GameProcess.Actors.Server
{
    public class MovesActor : IActor, IServerPacketHandler
    {
        GameStateServer gameState;
        CollisionActor _collisionsActor;

        public MovesActor(GameStateServer gameState, CollisionActor collisionsActor)
        {
            this.gameState = gameState;
            this._collisionsActor = collisionsActor;
        }

        public void Update(GameTime gameTime)
        {
            // формирование данных для сообщений клиентам
            lock (gameState.Bots)
            {
                foreach (var bot in gameState.Bots)
                {
                    Vector2 prevPos = bot.Position;
                    bot.Move(gameTime);
                    _collisionsActor.CheckForCollisions(bot);
                    if (bot.Position != prevPos)
                    {
                        NotifyPlayersCharacterMoved(bot);
                    }
                }
            }
        }

        private void NotifyPlayersCharacterMoved(RealCharacter character)
        {
            foreach (var netplayer in gameState.NetPlayers.Values)
            {
                netplayer.ServerPacket.PlayersPositions[character.Id] = (ProtoVector2)character.Position;
            }
        }

        public void Receive(ClientPacket msg)
        {
            // обрабатываем полученное сообщение
            RealCharacter player = gameState.Players[msg.PlayerId];
            if (msg.Moves.Count > 0)
            {
                Vector2 prevPos = player.Position;
                player.MoveBehavior.NewPosition = player.Position;
                foreach (var moves in msg.Moves)
                {
                    // игнорируем передвижения, которые уже были обработаны
                    if (moves.Key <= gameState.NetPlayers[msg.PlayerId].ServerPacket.Ack)
                    {
                        continue;
                    }
                    foreach (var move in moves.Value)
                    {
                        player.MoveBehavior.NewPosition += move.Direction * (float)(player.Speed * move.Time.TotalSeconds);
                    }
                }
                _collisionsActor.CheckForCollisions(player);
                if (player.Position != prevPos)
                {
                    NotifyPlayersCharacterMoved(player);
                }
            }
        }
    }
}
