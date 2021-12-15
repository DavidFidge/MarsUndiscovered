using FrigidRogue.MonoGame.Core.Graphics.Quads;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Interfaces
{
    public interface IAssets
    {
        public Texture2D TitleTexture { get; set; }
        public SpriteFont MapFont { get; set; }
        public MaterialQuadTemplate WallBackgroundQuad { get; set; }
        public TexturedQuadTemplate WallForegroundQuad { get; set; }
        public TexturedQuadTemplate FloorQuad { get; set; }
        Effect TextureMaterialEffect { get; set; }

        void LoadContent();
    }
}