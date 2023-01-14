using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

using Point = SadRogue.Primitives.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapTileEntity : Entity, ISpriteBatchDrawable
    {
        public IAssets Assets { get; set; }
        public Point Position { get; set; }
        public MapTileTexture MapTileTexture { get; set; }
        public bool IsVisible { get; set; }
        public float? Opacity { get; set; }

        public MapTileEntity(Point position)
        {
            Position = position;
            Transform.ChangeTranslation(
                new Vector3(Position.X * Constants.TileQuadWidth, -Position.Y * Constants.TileQuadHeight, 0)
            );
        }

        public void SetWall()
        {
            MapTileTexture = Assets.Wall;
            IsVisible = true;
        }

        public void SetFloor()
        {
            MapTileTexture = Assets.Floor;
            IsVisible = true;
        }

        public void SetPlayer()
        {
            MapTileTexture = Assets.Player;
            IsVisible = true;
        }

        public void SetMapExit(Direction direction)
        {
            MapTileTexture = direction == Direction.Down ? Assets.MapExitDown : Assets.MapExitUp;

            IsVisible = true;
        }

        public void SetShip(char shipPart)
        {
            MapTileTexture = Assets.ShipParts[shipPart];
            IsVisible = true;
        }
        
        public void SetMiningFacility(char miningFacilityPart)
        {
            MapTileTexture = Assets.MiningFacilitySection[miningFacilityPart];
            IsVisible = true;
        }

        public void SetItem(ItemType itemType)
        {
            switch (itemType)
            {
                case Weapon _:
                    MapTileTexture = Assets.Weapon;
                    break;

                case Gadget _:
                    MapTileTexture = Assets.Gadget;
                    break;

                case NanoFlask _:
                    MapTileTexture = Assets.NanoFlask;
                    break;

                case ShipRepairParts _:
                    MapTileTexture = Assets.ShipRepairParts;
                    break;
            }

            IsVisible = true;
        }

        public void SetMonster(Breed breed)
        {
            MapTileTexture = Assets.Monsters[breed.Name];
           
            IsVisible = true;
        }

        public void SetMouseHover()
        {
            MapTileTexture = Assets.MouseHover;
            IsVisible = false;
        }
        
        public void SpriteBatchDraw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                var drawRegion = new Rectangle(Position.X * Constants.TileWidth, Position.Y * Constants.TileHeight,
                    Constants.TileWidth, Constants.TileHeight);

                MapTileTexture.SpriteBatchDraw(spriteBatch, drawRegion, Opacity);
            }
        }

        public void SetLightning(float opacity)
        {
            IsVisible = true;
            MapTileTexture = Assets.Lightning;
            Opacity = opacity;
        }
        
        public void SetLineAttack(Direction direction)
        {
            IsVisible = true;

            switch (direction.Type)
            {
                case Direction.Types.None:
                    break;
                case Direction.Types.Up:
                case Direction.Types.Down:
                    MapTileTexture = Assets.LineAttackNorthSouth;
                    break;
                case Direction.Types.UpRight:
                case Direction.Types.DownLeft:
                    MapTileTexture = Assets.LineAttackNorthEastSouthWest;
                    break;
                case Direction.Types.Right:
                case Direction.Types.Left:
                    MapTileTexture = Assets.LineAttackEastWest;
                    break;
                case Direction.Types.DownRight:
                case Direction.Types.UpLeft:
                    MapTileTexture = Assets.LineAttackNorthWestSouthEast;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}