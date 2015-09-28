using System;
using Microsoft.Xna.Framework;
using GameObjects;
using GameObjects.DrawableClasses;

namespace GameProcess.Actors
{
    public class CollisionActor
    {
        readonly GameState gameState;

        public CollisionActor(GameState gameState)
        {
            this.gameState = gameState;
            gameState.CharacterAdded += new Action<RealCharacter>(OnCharacterAdd);
        }

        private void OnCharacterAdd(RealCharacter character)
        {
            gameState.Grid.Put(character);
        }

        public void CheckForCollisions(RealCharacter character)
        {
            if (character.MoveBehavior.NewPosition == character.Position)
                return;
            gameState.Level.CurrectNewPositionIfOut(character);
            gameState.Grid.Relocate(character);
            CheckNewPositionForCollisionsWithMapObjects(character);
            character.Position = character.MoveBehavior.NewPosition;
        }

        private void CheckNewPositionForCollisionsWithMapObjects(RealCharacter character)
        {
            Point newZone = gameState.Grid.GetZone(character.MoveBehavior.NewPosition);
            for (int i = newZone.X - 1; i <= newZone.X + 1; i++)
            {
                for (int j = newZone.Y - 1; j <= newZone.Y + 1; j++)
                {
                    if (i < 0 || j < 0 || i >= gameState.Grid.Solids.Count || j >= gameState.Grid.Solids[i].Count)
                    {
                        continue;
                    }
                    foreach (var solid in gameState.Grid.Solids[i][j])
                    {
                        DeepCorrection(character, solid);
                    }
                }
            }
        }

        private void DeepCorrection(Character character, Solid solid)
        {
            Vector2 newPos = character.MoveBehavior.NewPosition;
            float charRectLeft = newPos.X - (character.CollisionOffset.X >> 1);
            float charRectTop = newPos.Y - (character.CollisionOffset.Y >> 1);
            float charRectRight = newPos.X + (character.CollisionOffset.X >> 1);
            float charRectBottom = newPos.Y + (character.CollisionOffset.Y >> 1);
            Vector2 minDelta = Vector2.Zero;
            float minDeltaLength = 0;
            Vector2[] axes = new Vector2[solid.Axes.Length + 2];
            axes[0] = new Vector2(1, 0);
            axes[1] = new Vector2(0, 1);
            solid.Axes.CopyTo(axes, 2);
            foreach (var axis in axes)
            {
                // проекция персонажа
                Vector2 projBegin1;
                projBegin1 = GetProjection(new Vector2(charRectLeft, charRectTop), axis);
                Vector2 proj = GetProjection(new Vector2(charRectLeft, charRectBottom), axis);
                if ((proj.X < projBegin1.X) || ((proj.X == projBegin1.X) && (proj.Y < projBegin1.Y)))
                    projBegin1 = proj;
                Vector2 projEnd1;
                projEnd1 = GetProjection(new Vector2(charRectRight, charRectTop), axis);
                proj = GetProjection(new Vector2(charRectRight, charRectBottom), axis);
                if ((proj.X > projEnd1.X) || ((proj.X == projEnd1.X) && (proj.Y > projEnd1.Y)))
                    projEnd1 = proj;
                // проекция тела
                Vector2 projBegin2;
                Vector2 projEnd2;
                projBegin2 = projEnd2 = GetProjection(solid.Vertices[0], axis);
                for (int i = 1; i < solid.Vertices.Length; i++)
                {
                    proj = GetProjection(solid.Vertices[i], axis);
                    if ((proj.X < projBegin2.X) || ((proj.X == projBegin2.X) && (proj.Y < projBegin2.Y)))
                        projBegin2 = proj;
                    else if ((proj.X > projEnd2.X) || ((proj.X == projEnd2.X) && (proj.Y > projEnd2.Y)))
                        projEnd2 = proj;
                }
                Vector2 delta = Vector2.Zero;
                Vector2 d1 = projEnd1 - projBegin2;
                if ((d1.X > 0) || ((d1.X == 0) && (d1.Y > 0)))
                {
                    Vector2 d2 = projEnd2 - projBegin1;
                    if ((d2.X > 0) || ((d2.X == 0) && (d2.Y > 0)))
                    {
                        if ((d1.X < d2.X) || ((d1.X == d2.X) && (d1.Y <= d2.Y)))
                        {
                            delta = -d1;
                        }
                        else
                        {
                            delta = d2;
                        }
                    }
                }
                if (delta == Vector2.Zero)
                {
                    return;
                }
                float deltaLength = delta.Length();
                if ((minDelta == Vector2.Zero) || (deltaLength < minDeltaLength))
                {
                    minDelta = delta;
                    minDeltaLength = deltaLength;
                }
            }
            character.MoveBehavior.NewPosition += minDelta;
        }
        /// <summary>
        /// Возвращает проекцию вектора a на вектор b.
        /// </summary>
        /// <param name="a">Проецируемый вектор.</param>
        /// <param name="b">Вектор оси, на которую проводится проекция.</param>
        /// <returns>Проекция.</returns>
        private Vector2 GetProjection(Vector2 a, Vector2 b)
        {
            return (a.X * b.X + a.Y * b.Y) * b;
        }
    }
}
