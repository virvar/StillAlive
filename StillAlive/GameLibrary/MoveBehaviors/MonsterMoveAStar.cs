using Microsoft.Xna.Framework;
using System;
using GameObjects.Helpers;

namespace GameObjects
{
    public class MonsterMoveAStar : StupidMonsterMove
    {
        private const int _tileSize = 32;
        private AStar _AStar;
        private int[,] _grid;

        public MonsterMoveAStar(Character character, Level level, params Character[] targets)
            : base(character, targets)
        {
            _grid = GetSimplifiedGrid(level);
            _AStar = new AStar();
        }

        private int[,] GetSimplifiedGrid(Level level)
        {
            var height = (int)Math.Ceiling((double)level.Height / _tileSize);
            var width = (int)Math.Ceiling((double)level.Width / _tileSize);
            var solids = new int[width, height];
            foreach (var solid in level.Solids)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (CollisionHelper.Intersects(new Rectangle(x * _tileSize, y * _tileSize, _tileSize, _tileSize), solid))
                        {
                            solids[x, y] = 1;
                        }
                    }
                }
            }
            return solids;
        }

        public override void Move(GameTime gameTime)
        {
            if (Targets.Count != 0)
            {
                // find nearest target
                Vector2 distance = (Targets[0].Position - Character.Position);
                Character closestTarget = Targets[0];
                for (int i = 0; i < Targets.Count; i++)
                {
                    Vector2 tmpDistance = (Targets[i].Position - Character.Position);
                    if (tmpDistance.Length() < distance.Length())
                    {
                        distance = tmpDistance;
                        closestTarget = Targets[i];
                    }
                }
                var start = GetPositionOnGrid(Character);
                var end = GetPositionOnGrid(closestTarget);

                var path = _AStar.GetPath(gameTime, _grid, start, end);
                var nextPoint = _AStar.NextPoint(gameTime, _grid, start, end);
                var direction = path.Count > 1 ?
                    (nextPoint.ToVector2() * _tileSize + new Vector2(_tileSize / 2, _tileSize / 2) - Character.Position)
                    : Vector2.Zero;

                //var path = _AStar.GetPath(_grid, start, end);
                //var direction = path.Count > 1 ?
                //    (path[1].ToVector2() * _tileSize + new Vector2(_tileSize / 2, _tileSize / 2) - Character.Position)
                //    : Vector2.Zero;

                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                }
                float delta = (float)(Character.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                var move = direction * delta;
                NewPosition = Character.Position + move;
            }
        }

        public Point GetPositionOnGrid(Character character)
        {
            int newZoneX = (int)Math.Floor(character.Position.X / _tileSize);
            int newZoneY = (int)Math.Floor(character.Position.Y / _tileSize);
            return new Point(newZoneX, newZoneY);
        }
    }
}
