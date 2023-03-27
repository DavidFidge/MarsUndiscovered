using FrigidRogue.MonoGame.Core.Graphics.Quads;
using GoRogue.GameFramework;
using MarsUndiscovered.Components;
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
        IDictionary<char, MapTileTexture> ShipParts { get; set; }
        IDictionary<char, MapTileTexture> MiningFacilitySection { get; set; }
        SpriteFont UiRegularFont { get; set; }

        void LoadContent();
        SpriteSheet GetRadioCommsSpriteSheet(IGameObject gameObject);
        IMapTileTexture GetMapTileTexture(TileAnimationType tileAnimationType);
        IMapTileTexture GetMapTileTexture(Breed breed);
        IMapTileTexture GetMapTileTexture(ItemType itemType);
        Texture2D GetStaticTexture(TileAnimationType tileAnimationType);
        Texture2D GetStaticTexture(Breed breed);
        Texture2D GetStaticTexture(ItemType itemType);
    }
}
