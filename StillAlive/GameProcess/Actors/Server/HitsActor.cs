using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virvar.Net;
using Microsoft.Xna.Framework;
using GameObjects.DrawableClasses;
using GameObjects;
using GameObjects.ProtoClasses;

namespace GameProcess.Actors.Server
{
    class HitsActor : IActor, IServerPacketHandler
    {
        GameStateServer _gameState;

        public HitsActor(GameStateServer gameState)
        {
            this._gameState = gameState;
        }
        
        public void Update(GameTime gameTime)
        {
            // регенерация здоровья и маны
            foreach (var player in _gameState.Players.Values)
            {
                player.Health += (float)(player.HealthRegen * gameTime.ElapsedGameTime.TotalSeconds);
                player.Mana += (float)(player.ManaRegen * gameTime.ElapsedGameTime.TotalSeconds);
                if (player.Health > Character.MaxHealth)
                {
                    player.Health = Character.MaxHealth;
                }
                if (player.Mana > Character.MaxMana)
                {
                    player.Mana = Character.MaxMana;
                }
                // сообщаем всем игрокам состояние маны стреляющего игрока
                foreach (var netPlayer in _gameState.NetPlayers.Values)
                {
                    netPlayer.ServerPacket.PlayersHealths[(byte)player.Id] = (byte)player.Health;
                    netPlayer.ServerPacket.PlayersMana[(byte)player.Id] = (byte)player.Mana;
                }
            }
            foreach (var bot in _gameState.Bots)
            {
                bot.Health += (float)(bot.HealthRegen * gameTime.ElapsedGameTime.TotalSeconds);
                bot.Mana += (float)(bot.ManaRegen * gameTime.ElapsedGameTime.TotalSeconds);
                if (bot.Health > Character.MaxHealth)
                {
                    bot.Health = Character.MaxHealth;
                }
                if (bot.Mana > Character.MaxMana)
                {
                    bot.Mana = Character.MaxMana;
                }
                // сообщаем всем игрокам состояние маны стреляющего игрока
                foreach (var netPlayer in _gameState.NetPlayers.Values)
                {
                    netPlayer.ServerPacket.PlayersHealths[(byte)bot.Id] = (byte)bot.Health;
                    netPlayer.ServerPacket.PlayersMana[(byte)bot.Id] = (byte)bot.Mana;
                }
            }
        }
        
        public void Receive(ClientPacket msg)
        {
            Character shooter = _gameState.Players[msg.PlayerId];
            foreach (var shoot in msg.Shoots)
            {
                // игнорируем выстрелы, которые уже были обработаны
                if (_gameState.NetPlayers[msg.PlayerId].ServerPacket.Ack >= shoot.Key)
                {
                    continue;
                }
                float shootingTime = shoot.Value;
                float damage = shootingTime * shooter.DamagePerSecond;
                if (shooter.Mana < damage)
                    break;
                shooter.Mana -= damage;
                // сообщаем всем игрокам состояние маны стреляющего игрока
                foreach (var netPlayer in _gameState.NetPlayers.Values)
                {
                    netPlayer.ServerPacket.PlayersMana[(byte)shooter.Id] = (byte)shooter.Mana;
                }
                if (shooter.IsShooting = (shootingTime != 0))
                {
                    float wrapedAngle = MathHelper.WrapAngle(-MathHelper.PiOver2 + shooter.Angle);
                    double koef = -Math.Tan(wrapedAngle); // тангенс угла наклона
                    Vector2 direction = new Vector2((float)Math.Cos(wrapedAngle), (float)Math.Sin(wrapedAngle));
                    CheckHits(shooter, direction, shootingTime);
                }
            }
            // сообщаем всем игрокам, что игрок стреляет
            foreach (var netPlayer in _gameState.NetPlayers.Values)
            {
                lock (netPlayer.ServerPacket.ShootingPlayers)
                {
                    if (shooter.IsShooting)
                    {
                        netPlayer.ServerPacket.ShootingPlayers.Add(msg.PlayerId);
                    }
                    else
                    {
                        netPlayer.ServerPacket.ShootingPlayers.Remove(msg.PlayerId);
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает точку пересечения луча с объектом.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="koef"></param>
        /// <param name="shooter"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector2 GetHitPosition(Vector2 direction, Character shooter, ISolid target)
        {
            Vector2 hitPosition = new Vector2(-1);
            Vector2 p0 = shooter.Position;
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            // пересечение с отрезками полигона
            Vector2[] vertices = target.Vertices;
            Vector2 prevV = vertices[vertices.Length - 1];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 curV = vertices[i];
                Vector2 intr = GetIntersectionPosition(shooter.Position, direction, curV, curV - prevV);
                float x = intr.X;
                float y = intr.Y;
                // точка пересечения прямых в пределах отрезка
                if (((x >= prevV.X) && (x < curV.X)) ||
                    ((x >= curV.X) && (x < prevV.X)) ||
                    ((y >= prevV.Y) && (y < curV.Y)) ||
                    ((y >= curV.Y) && (y < prevV.Y)))
                {
                    // точка пересечения в направлении стрельбы
                    Vector2 delta = new Vector2(x - p0.X, y - p0.Y);
                    if (((delta.X >= 0 && direction.X >= 0) || (delta.X < 0 && direction.X < 0)) &&
                        ((delta.Y >= 0 && direction.Y >= 0) || (delta.Y < 0 && direction.Y < 0)))
                    {
                        if (delta.X != 0)
                        {
                            float positiveDeltaX = Math.Abs(delta.X);
                            if (positiveDeltaX < minX)
                            {
                                minX = positiveDeltaX;
                                hitPosition = new Vector2((float)x, (float)y);
                            }
                        }
                        else
                        {
                            float positiveDeltaY = Math.Abs(delta.Y);
                            if (positiveDeltaY < minY)
                            {
                                minY = positiveDeltaY;
                                hitPosition = new Vector2((float)x, (float)y);
                            }
                        }
                    }
                }
                prevV = curV;
            }
            return hitPosition;
        }

        private void CheckHits(Character shooter, Vector2 direction, float shootingTime)
        {
            int voxelSize = Level.ZoneSize;
            // координаты ячейки, в которой находится начало луча
            int X = (int)Math.Floor(shooter.Position.X / voxelSize);
            int Y = (int)Math.Floor(shooter.Position.Y / voxelSize);
            // шаг по ячейкам в направлении луча
            int stepX = (direction.X > 0) ? 1 : -1;
            int stepY = (direction.Y > 0) ? 1 : -1;
            // расстояние вдоль луча до первой вертикальной линии, которую пересекает луч
            Vector2 horIntersection = GetIntersectionPosition(shooter.Position, direction, new Vector2((X + (stepX > 0 ? 1 : 0)) * voxelSize, 0), new Vector2(0, 1)) - shooter.Position;
            float tMaxX = horIntersection.Length();
            // расстояние вдоль луча до первой горизонтальной линии, которую пересекает луч
            Vector2 verIntersection = GetIntersectionPosition(shooter.Position, direction, new Vector2(0, (Y + (stepY > 0 ? 1 : 0)) * voxelSize), new Vector2(1, 0)) - shooter.Position;
            float tMaxY = verIntersection.Length();
            // расстояние вдоль луча, которое нужно пройти, чтобы заполнить ширину ячейки
            Vector2 fillWidth = GetIntersectionPosition(Vector2.Zero, direction, new Vector2(voxelSize, 0), new Vector2(0, 1));
            float tDeltaX = fillWidth.Length();
            // расстояние вдоль луча, которое нужно пройти, чтобы заполнить высоту ячейки
            Vector2 fillHeigth = GetIntersectionPosition(Vector2.Zero, direction, new Vector2(0, voxelSize), new Vector2(1, 0));
            float tDeltaY = fillHeigth.Length();
            // обоходим последовательно ячейки, которые пересекает луч
            while (true)
            {
                if (CheckVoxel(shooter, shootingTime, direction, new Point(X, Y)))
                {
                    break;
                }
                if (tMaxX < tMaxY)
                {
                    tMaxX += tDeltaX;
                    X += stepX;
                    if (X < 0 || X >= _gameState.Grid.Width)
                    {
                        break;
                    }
                }
                else
                {
                    tMaxY += tDeltaY;
                    Y += stepY;
                    if (Y < 0 || Y >= _gameState.Grid.Height)
                    {
                        break;
                    }
                }
            }
        }
        
        private Vector2 GetIntersectionPosition(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2)
        {
            // случаи, когда линии совпадают, игнорируем
            float x;
            float y;
            if (v1.X == 0)
            {
                x = p1.X;
                float A2 = -v2.Y / v2.X;
                float C2 = A2 * p2.X + p2.Y;
                y = C2 - A2 * x;
            }
            else if (v1.Y == 0)
            {
                y = p1.Y;
                float A2 = -v2.Y / v2.X;
                float C2 = A2 * p2.X + p2.Y;
                x = (C2 - y) / A2;
            }
            else if (v2.X == 0)
            {
                x = p2.X;
                float A1 = -v1.Y / v1.X;
                float C1 = A1 * p1.X + p1.Y;
                y = C1 - A1 * x;
            }
            else if (v2.Y == 0)
            {
                y = p2.Y;
                float A1 = -v1.Y / v1.X;
                float C1 = A1 * p1.X + p1.Y;
                x = (C1 - y) / A1;
            }
            else
            {
                float A1 = -v1.Y / v1.X;
                float C1 = A1 * p1.X + p1.Y;
                float A2 = -v2.Y / v2.X;
                float C2 = A2 * p2.X + p2.Y;
                x = (C1 - C2) / (A1 - A2);
                y = (A1 * C2 - A2 * C1) / (A1 - A2);
            }
            return new Vector2((float)x, (float)y);
        }

        private bool CheckVoxel(Character shooter, float shootingTime, Vector2 direction, Point voxel)
        {
            bool result = false;
            // проверяем попадания в объекты
            // зона попадания
            Rectangle hitArea = new Rectangle(voxel.X * Level.ZoneSize, voxel.Y * Level.ZoneSize, (voxel.X + 1) * Level.ZoneSize, (voxel.Y + 1) * Level.ZoneSize);
            foreach (var solid in _gameState.Grid.Solids[voxel.X][voxel.Y])
            {
                Vector2 hitPos = GetHitPosition(direction, shooter, solid);
                if (hitPos.X != -1)
                {
                    if (hitPos.X < shooter.Position.X && hitPos.X > hitArea.X)
                    {
                        hitArea.X = (int)hitPos.X;
                        result = true;
                    }
                    if (hitPos.X > shooter.Position.X && hitPos.X < hitArea.Width)
                    {
                        hitArea.Width = (int)hitPos.X;
                        result = true;
                    }
                    if (hitPos.Y < shooter.Position.Y && hitPos.Y > hitArea.Y)
                    {
                        hitArea.Y = (int)hitPos.Y;
                        result = true;
                    }
                    if (hitPos.Y > shooter.Position.Y && hitPos.Y < hitArea.Height)
                    {
                        hitArea.Height = (int)hitPos.Y;
                        result = true;
                    }
                }
            }
            // проверяем попадания в игроков
            Character[] playersInVoxel = _gameState.Grid.Characters[voxel.X][voxel.Y].ToArray();
            foreach (var player in playersInVoxel)
            {
                if (player != shooter)
                {
                    if (((player.Position.X >= shooter.Position.X && player.Position.X <= hitArea.Width) ||
                        (player.Position.X <= shooter.Position.X && player.Position.X >= hitArea.X))
                        &&
                        ((player.Position.Y >= shooter.Position.Y && player.Position.Y <= hitArea.Height) ||
                        (player.Position.Y <= shooter.Position.Y && player.Position.Y >= hitArea.Y)))
                    {
                        Vector2 hitPos = GetHitPosition(direction, shooter, player);
                        if (hitPos.X != -1)
                        {
                            ApplyHit(shooter, player, shootingTime);
                        }
                    }
                }
            }
            return result;
        }

        private void ApplyHit(Character shooter, Character player, float shootingTime)
        {
            player.Health -= shootingTime * shooter.DamagePerSecond;
            if (player.Health <= 0)
            {
                // добавляем игрока в точку респауна зону и удаляем с зоны, в которой он был
                int x = (int)Math.Floor(player.Position.X / Level.ZoneSize);
                int y = (int)Math.Floor(player.Position.Y / Level.ZoneSize);
                _gameState.Grid.Characters[x][y].Remove(player);
                player.Position = _gameState.Level.GetRespawnPosition();
                int newX = (int)Math.Floor(player.Position.X / Level.ZoneSize);
                int newY = (int)Math.Floor(player.Position.Y / Level.ZoneSize);
                _gameState.Grid.Characters[newX][newY].Add(player);
                player.Health = Character.MaxHealth;
                _gameState.Scores[shooter.Id].Kills++;
                _gameState.Scores[player.Id].Deaths++;
                foreach (var netPlayer in _gameState.NetPlayers.Values)
                {
                    netPlayer.ServerPacket.PlayersPositions[player.Id] = (ProtoVector2)player.Position;
                    netPlayer.ServerPacket.PlayersHealths[(byte)player.Id] = (byte)player.Health;
                }
            }
            else
            {
                foreach (var netPlayer in _gameState.NetPlayers.Values)
                {
                    netPlayer.ServerPacket.PlayersHealths[(byte)player.Id] = (byte)player.Health;
                }
            }
        }
    }
}
