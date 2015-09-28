using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameObjects.MoveBehaviors
{
    public class EmptyBehavior : IMoveBehavior
    {
        private Character _character;
        public Character Character
        {
            get { return _character; }
            set { _character = value; }
        }
        public Vector2 NewPosition { get; set; }

        public EmptyBehavior(Character character)
        {
            this._character = character;
        }

        public void Move(GameTime gameTime)
        {
            // ничего не делает
            NewPosition = _character.Position;
        }
    }
}
