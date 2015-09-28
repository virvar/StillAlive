using Microsoft.Xna.Framework;

namespace GameObjects.DrawableClasses
{
    public class RealCharacter : DrawableCharacter
    {
        internal Rectangle CollisionRect { get; set; }

        public RealCharacter(int id)
            : base(id)
        {
            CollisionRect = new Rectangle(0, 0, 32, 32);
        }

        public bool IsCollide(RealCharacter character2)
        {
            return CollisionRect.Intersects(character2.CollisionRect);
        }
    }
}
