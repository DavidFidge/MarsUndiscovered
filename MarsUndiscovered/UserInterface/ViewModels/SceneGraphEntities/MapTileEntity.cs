using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Components;
using MarsUndiscovered.Graphics;
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
        public IMapTileTexture MapTileTexture { get; set; }
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
            MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.Wall);
            IsVisible = true;
        }

        public void SetFloor()
        {
            MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.Floor);
            IsVisible = true;
        }

        public void SetPlayer()
        {
            MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.Player);
            IsVisible = true;
        }

        public void SetMapExit(Direction direction)
        {
            MapTileTexture = Assets.GetMapTileTexture(direction == Direction.Down ? TileAnimationType.MapExitDown : TileAnimationType.MapExitUp);
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
            MapTileTexture = Assets.GetMapTileTexture(itemType);
            IsVisible = true;
        }

        public void SetMonster(Breed breed)
        {
            MapTileTexture = Assets.GetMapTileTexture(breed);
            IsVisible = true;
        }

        public void SetMouseHover()
        {
            MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.MouseHover);
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
            MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.Lightning);
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
                    MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.LineAttackNorthSouth);
                    break;
                case Direction.Types.UpRight:
                case Direction.Types.DownLeft:
                    MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.LineAttackNorthEastSouthWest);
                    break;
                case Direction.Types.Right:
                case Direction.Types.Left:
                    MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.LineAttackEastWest);
                    break;
                case Direction.Types.DownRight:
                case Direction.Types.UpLeft:
                    MapTileTexture = Assets.GetMapTileTexture(TileAnimationType.LineAttackNorthWestSouthEast);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}