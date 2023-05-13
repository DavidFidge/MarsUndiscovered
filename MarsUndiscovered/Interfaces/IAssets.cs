using FrigidRogue.MonoGame.Core.Graphics.Quads;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MarsUndiscovered.Interfaces
{
    public interface IAssets
    {
        public Texture2D TitleTexture { get; set; }
        public Texture2D TitleTextTexture { get; set; }
        GoalMapTileTexture GoalMapTileTexture { get; set; }
        SpriteFont UiRegularFont { get; set; }

        void LoadContent();
        SpriteSheet GetRadioCommsSpriteSheet(RadioCommsTypes radioCommsType);
        IMapTileTexture GetMapTileTexture(TileGraphicType tileGraphicType);
        IMapTileTexture GetMapTileTexture(TileGraphicType tileGraphicType, string additionalKey);
        Texture2D GetStaticTexture(TileGraphicType tileGraphicType);
        Texture2D GetStaticTexture(string key);
        void Update();
        void SetTileGraphicOptions(TileGraphicOptions tileGraphicOptions);
    }
}
