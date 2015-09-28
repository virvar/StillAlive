using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProcess.Actors
{
    public interface IActor
    {
        void Update(Microsoft.Xna.Framework.GameTime gameTime);
    }
}
