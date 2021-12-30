using FrigidRogue.MonoGame.Core.Graphics.Quads;

using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Interfaces
{
    public interface IAssets
    {
        public Texture2D TitleTexture { get; set; }
        MapTileQuad Wall { get; set; }
        MapTileQuad Floor { get; set; }
        MapTileQuad Player { get; set; }
        MapTileQuad Roach { get; set; }

        void LoadContent();
    }
}