using System;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Quads;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework;

using SadRogue.Primitives;

using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapTileEntity : Entity, IDrawable
    {
        public IAssets Assets { get; set; }
        public Point Position { get; set; }
        public MapTileQuad MapTileQuad { get; set; }
        public bool IsVisible { get; set; }
        public float BackgroundOpacity = -1f;

        public MapTileEntity(Point position)
        {
            Position = position;
            Transform.ChangeTranslation(
                new Vector3(Position.X * Graphics.Assets.TileQuadWidth, -Position.Y * Graphics.Assets.TileQuadHeight, 0)
            );
        }

        public void SetWall()
        {
            MapTileQuad = Assets.Wall;
            IsVisible = true;
        }

        public void SetFloor()
        {
            MapTileQuad = Assets.Floor;
            IsVisible = true;
        }

        public void SetPlayer()
        {
            MapTileQuad = Assets.Player;
            IsVisible = true;
        }

        public void SetMapExit(Direction direction)
        {
            MapTileQuad = direction == Direction.Down ? Assets.MapExitDown : Assets.MapExitUp;

            IsVisible = true;
        }

        public void SetShip(char shipPart)
        {
            MapTileQuad = Assets.ShipParts[shipPart];
            IsVisible = true;
        }

        public void SetItem(ItemType itemType)
        {
            switch (itemType)
            {
                case Weapon _:
                    MapTileQuad = Assets.Weapon;
                    break;

                case Gadget _:
                    MapTileQuad = Assets.Gadget;
                    break;

                case NanoFlask _:
                    MapTileQuad = Assets.NanoFlask;
                    break;

                case ShipRepairParts _:
                    MapTileQuad = Assets.ShipRepairParts;
                    break;
            }

            IsVisible = true;
        }

        public void SetMonster(Breed breed)
        {
            MapTileQuad = Assets.Monsters[breed.Name];
           
            IsVisible = true;
        }

        public void SetMouseHover()
        {
            MapTileQuad = Assets.MouseHover;
            IsVisible = false;
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (!IsVisible)
                return;

            if (BackgroundOpacity >= 0f)
                MapTileQuad.Draw(view, projection, world, BackgroundOpacity);
            else
                MapTileQuad.Draw(view, projection, world);
        }

        public void SetLightning(float opacity)
        {
            IsVisible = true;
            MapTileQuad = Assets.Lightning;
            BackgroundOpacity = opacity;
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
                    MapTileQuad = Assets.LineAttackNorthSouth;
                    break;
                case Direction.Types.UpRight:
                case Direction.Types.DownLeft:
                    MapTileQuad = Assets.LineAttackNorthEastSouthWest;
                    break;
                case Direction.Types.Right:
                case Direction.Types.Left:
                    MapTileQuad = Assets.LineAttackEastWest;
                    break;
                case Direction.Types.DownRight:
                case Direction.Types.UpLeft:
                    MapTileQuad = Assets.LineAttackNorthWestSouthEast;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}