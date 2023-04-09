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
        SpriteSheet GetRadioCommsSpriteSheet(IGameObject gameObject);
        IMapTileTexture GetMapTileTexture(TileGraphicType tileGraphicType);
        IMapTileTexture GetMapTileTexture(Breed breed);
        IMapTileTexture GetMapTileTexture(ItemType itemType);
        Texture2D GetStaticTexture(TileGraphicType tileGraphicType);
        Texture2D GetStaticTexture(Breed breed);
        Texture2D GetStaticTexture(ItemType itemType);
        IMapTileTexture GetMapTileTexture(TileGraphicFeatureType tileGraphicFeatureType, char c);
        void Update();
    }
}
