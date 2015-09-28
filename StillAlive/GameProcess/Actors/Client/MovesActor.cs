using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects.DrawableClasses;
using Microsoft.Xna.Framework;
using Virvar.Net;
using GameObjects.ProtoClasses;

namespace GameProcess.Actors.Client
{
    public class MovesActor : IActor, IClientPacketHandler
    {
        private GameStateClient _gameState;
        private CollisionActor _collisionsActor;

        public MovesActor(GameStateClient gameState, CollisionActor collisionsActor)
        {
            this._gameState = gameState;
            this._collisionsActor = collisionsActor;
        }

        public void Update(GameTime gameTime)
        {
            lock (_gameState.NetPlayer.ClientPacket.Moves)
            {
                Vector2 delta = _gameState.Player.Position;
                _gameState.Player.Move(gameTime);
                _collisionsActor.CheckForCollisions(_gameState.Player);
                delta = _gameState.Player.Position - delta;
                if (delta != Vector2.Zero)
                {
                    float moveLength = (float)(_gameState.Player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                    Vector2 direction = new Vector2()
                    {
                        X = delta.X / moveLength,
                        Y = delta.Y / moveLength
                    };
                    AddMoveToClientPacket(direction, gameTime);
                }
            }
        }

        private void AddMoveToClientPacket(Vector2 direction, GameTime gameTime)
        {
            ushort sequence = _gameState.NetPlayer.ClientPacket.Sequence;
            if (!_gameState.NetPlayer.ClientPacket.Moves.ContainsKey(sequence))
            {
                _gameState.NetPlayer.ClientPacket.Moves.TryAdd(sequence, new Moves());
            }
            _gameState.NetPlayer.ClientPacket.Moves[sequence].Add(new Move(direction, gameTime.ElapsedGameTime));
        }

        public void Receive(ServerPacket msg)
        {
            lock (_gameState.NetPlayer.ClientPacket.Moves)
            {
                DeleteProcessedMoves(msg);
                // обрабатываем пакет сервера на перемещения
                foreach (var playerPosition in msg.PlayersPositions)
                {
                    if (playerPosition.Key == _gameState.Player.Id)
                    {
                        _gameState.Player.Position = (Vector2)playerPosition.Value;
                        CalcPlayerLocalPosition(playerPosition.Value);
                        _collisionsActor.CheckForCollisions(_gameState.Player);
                    }
                    else
                    {
                        if (_gameState.Players.ContainsKey(playerPosition.Key))
                            _gameState.Players[playerPosition.Key].Position = (Vector2)playerPosition.Value;
                    }
                }
            }
        }

        private void DeleteProcessedMoves(ServerPacket msg)
        {
            List<ushort> keysToRemove = new List<ushort>();
            foreach (var moves in _gameState.NetPlayer.ClientPacket.Moves)
            {
                if (moves.Key <= msg.Ack)
                {
                    keysToRemove.Add(moves.Key);
                }
            }
            foreach (var key in keysToRemove)
            {
                Moves removedMoves;
                _gameState.NetPlayer.ClientPacket.Moves.TryRemove(key, out removedMoves);
            }
        }

        private void CalcPlayerLocalPosition(ProtoVector2 playerPosition)
        {
            _gameState.Player.MoveBehavior.NewPosition = _gameState.Player.Position;
            foreach (var moves in _gameState.NetPlayer.ClientPacket.Moves)
            {
                foreach (var move in moves.Value)
                {
                    _gameState.Player.MoveBehavior.NewPosition += move.Direction * (float)(_gameState.Player.Speed * move.Time.TotalSeconds);
                }
            }
        }
    }
}
