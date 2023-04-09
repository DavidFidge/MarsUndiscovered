using MarsUndiscovered.UserInterface.ViewModels;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public interface IMapTileEntityFactory
    {
        MapTileEntity Create(Point position);
        void Release(MapTileEntity mapTileEntity);
    }
}