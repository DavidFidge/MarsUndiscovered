using FrigidRogue.MonoGame.Core.Graphics.Quads;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Interfaces
{
    public interface IAssets
    {
        public Texture2D TitleTexture { get; set; }
        public SpriteFont MapFont { get; set; }
        public Texture2D Wall { get; set; }
        public TexturedQuadTemplate WallQuad { get; set; }

        public Texture2D Floor { get; set; }

        void LoadContent();
    }
}