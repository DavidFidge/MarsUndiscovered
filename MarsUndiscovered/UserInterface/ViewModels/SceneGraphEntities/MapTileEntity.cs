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
            }

            IsVisible = true;
        }

        public void SetMonster(Breed breed)
        {
            switch (breed)
            {
                case Roach _:
                    MapTileQuad = Assets.Roach;
                    break;
            }

            IsVisible = true;
        }

        public void SetMouseHover()
        {
            MapTileQuad = Assets.MouseHover;
            IsVisible = false;
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (IsVisible)
                MapTileQuad.Draw(view, projection, world);
        }
    }
}