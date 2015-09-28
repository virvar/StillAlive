using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects.DrawableClasses;
using Microsoft.Xna.Framework;

namespace GameProcess
{
    class GameMoment
    {
        public GameTime GameTime;
        public Dictionary<int, Vector2> CharactersPositions { get; set; }

        public GameMoment(GameTime gameTime, int charactersCount)
        {
            this.GameTime = gameTime;
            CharactersPositions = new Dictionary<int, Vector2>(charactersCount);
        }
        
        public void Add(IEnumerable<RealCharacter> characters)
        {
            foreach (var ch in characters)
            {
                CharactersPositions.Add(ch.Id, ch.Position);
            }
        }
    }
}
