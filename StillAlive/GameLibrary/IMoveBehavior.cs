using Microsoft.Xna.Framework;

namespace GameObjects
{
    public interface IMoveBehavior
    {
        Character Character { get; }
        Vector2 NewPosition { get; set; }

        void Move(GameTime gameTime);
    }
}
