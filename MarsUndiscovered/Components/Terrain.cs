using System;
using System.Collections.Generic;
using System.Linq;

namespace MarsUndiscovered.Components
{
    public class Terrain : MarsGameObject
    {
        public Terrain(bool isWalkable = true, bool isTransparent = true) : base(0, isWalkable, isTransparent)
        {
        }
    }
}