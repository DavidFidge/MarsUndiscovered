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
        MapTileQuad MapExitDown { get; set; }
        MapTileQuad MapExitUp { get; set; }
        MapTileQuad Roach { get; set; }
        GoalMapQuad GoalMapQuad { get; set; }
        MapTileQuad MouseHover { get; set; }
        MapTileQuad Weapon { get; set; }
        MapTileQuad Gadget { get; set; }
        MapTileQuad NanoFlask { get; set; }
        MapTileQuad FieldOfViewUnrevealedQuad { get; set; }
        MapTileQuad FieldOfViewHasBeenSeenQuad { get; set; }
        void LoadContent();
    }
}