using FrigidRogue.MonoGame.Core.Graphics.Quads;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MarsUndiscovered.Interfaces
{
    public interface IAssets
    {
        public Texture2D TitleTexture { get; set; }
        public Texture2D TitleTextTexture { get; set; }
        MapTileTexture Wall { get; set; }
        MapTileTexture Floor { get; set; }
        MapTileTexture Player { get; set; }
        MapTileTexture MapExitDown { get; set; }
        MapTileTexture MapExitUp { get; set; }
        IDictionary<string, MapTileTexture> Monsters { get; set; }
        GoalMapTileTexture GoalMapTileTexture { get; set; }
        MapTileTexture MouseHover { get; set; }
        MapTileTexture Weapon { get; set; }
        MapTileTexture Gadget { get; set; }
        MapTileTexture NanoFlask { get; set; }
        MapTileTexture Lightning { get; set; }
        MapTileTexture FieldOfViewUnrevealedTexture { get; set; }
        MapTileTexture FieldOfViewHasBeenSeenTexture { get; set; }
        IDictionary<char, MapTileTexture> ShipParts { get; set; }
        IDictionary<char, MapTileTexture> MiningFacilitySection { get; set; }
        MapTileTexture ShipRepairParts { get; set; }
        MapTileTexture LineAttackEastWest { get; set; }
        MapTileTexture LineAttackNorthEastSouthWest { get; set; }
        MapTileTexture LineAttackNorthWestSouthEast { get; set; }
        MapTileTexture LineAttackNorthSouth { get; set; }
        SpriteFont MapFont { get; set; }

        void LoadContent();
        SpriteSheet GetRadioCommsSpriteSheet(IGameObject gameObject);
    }
}
