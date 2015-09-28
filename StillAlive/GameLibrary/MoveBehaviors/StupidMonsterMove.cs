using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameObjects
{
    public class StupidMonsterMove : IMoveBehavior
    {
        public Character Character { get; set; }
        public Vector2 NewPosition { get; set; }
        protected List<Character> Targets;

        public StupidMonsterMove(Character character, params Character[] targets)
        {
            Character = character;
            Targets = new List<Character>(targets);
        }

        public void AddTarget(Character target)
        {
            Targets.Add(target);
        }

        public void RemoveTarget(Character target)
        {
            Targets.Remove(target);
        }

        public virtual void Move(GameTime gameTime)
        {
            if (Targets.Count != 0)
            {
                // ищем ближайшего врага
                // находим расстояние до ближайшего врага
                Vector2 distance = (Targets[0].Position - Character.Position);
                for (int i = 0; i < Targets.Count; i++)
                {
                    Vector2 tmpDistance = (Targets[i].Position - Character.Position);
                    if (tmpDistance.Length() < distance.Length())
                        distance = tmpDistance;
                }
                float delta = (float)(Character.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                distance.X = distance.X >= delta ? 1 : (distance.X <= -delta ? -1 : 0);
                distance.Y = distance.Y >= delta ? 1 : (distance.Y <= -delta ? -1 : 0);
                NewPosition = Character.Position + distance * delta;
            }
        }
    }
}
