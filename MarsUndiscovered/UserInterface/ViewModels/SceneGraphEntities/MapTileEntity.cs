using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using MarsUndiscovered.Game.Components;
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
                new Vector3(Position.X * UiConstants.TileQuadWidth, -Position.Y * UiConstants.TileQuadHeight, 0)
            );
        }

        public void SetWall(WallType wallType)
        {
            MapTileTexture = Assets.GetMapTileTexture(wallType.Name);
            IsVisible = true;
        }

        public void SetFloor(FloorType floorType)
        {
            MapTileTexture = Assets.GetMapTileTexture(floorType.Name);
            IsVisible = true;
        }
        
        public void SetDoor(DoorType doorType)
        {
            MapTileTexture = Assets.GetMapTileTexture(doorType.Name);
            IsVisible = true;
        }
        
        public void SetMachine(MachineType machineType)
        {
            MapTileTexture = Assets.GetMapTileTexture(machineType.Name);
            IsVisible = true;
        }

        public void SetEnvironmentalEffect(EnvironmentalEffectType environmentalEffectType)
        {
            MapTileTexture = Assets.GetMapTileTexture(environmentalEffectType.Name);
            IsVisible = true;
        }

        public void SetPlayer(Player player)
        {
            MapTileTexture = Assets.GetMapTileTexture(!player.IsDead ? TileGraphicType.Player.ToString() : TileGraphicType.PlayerDead.ToString());
            IsVisible = true;
        }

        public void SetMapExit(MapExitType mapExitType)
        {
            MapTileTexture = Assets.GetMapTileTexture(mapExitType.Name);
            IsVisible = true;
        }

        public void SetShip(char shipPart)
        {
            MapTileTexture = Assets.GetMapTileTexture($"{TileGraphicType.Ship}{shipPart}");
            IsVisible = true;
        }

        public void SetItem(ItemType itemType)
        {
            MapTileTexture = Assets.GetMapTileTexture(itemType.GetAbstractTypeName());
            IsVisible = true;
        }

        public void SetMonster(Breed breed)
        {
            MapTileTexture = Assets.GetMapTileTexture(breed.NameWithoutSpaces);
            IsVisible = true;
        }

        public void SetMouseHover()
        {
            MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.MouseHover.ToString());
            IsVisible = false;
        }
        
        public void SpriteBatchDraw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                var drawRegion = new Rectangle(Position.X * UiConstants.TileWidth, Position.Y * UiConstants.TileHeight,
                    UiConstants.TileWidth, UiConstants.TileHeight);

                MapTileTexture.SpriteBatchDraw(spriteBatch, drawRegion, Opacity);
            }
        }

        public void SetLightning(float opacity)
        {
            IsVisible = true;
            MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.Lightning.ToString());
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
                    MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.LineAttackNorthSouth.ToString());
                    break;
                case Direction.Types.UpRight:
                case Direction.Types.DownLeft:
                    MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.LineAttackNorthEastSouthWest.ToString());
                    break;
                case Direction.Types.Right:
                case Direction.Types.Left:
                    MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.LineAttackEastWest.ToString());
                    break;
                case Direction.Types.DownRight:
                case Direction.Types.UpLeft:
                    MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.LineAttackNorthWestSouthEast.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetLaser(float opacity)
        {
            IsVisible = true;
            MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.Laser.ToString());
            Opacity = opacity;
        }

        public void SetFeature(FeatureType featureType)
        {
            MapTileTexture = Assets.GetMapTileTexture(featureType.Name);
            IsVisible = true;
        }

        public void SetExplosion()
        {
            MapTileTexture = Assets.GetMapTileTexture(TileGraphicType.Explosion.ToString());
            IsVisible = true;
        }
    }
}