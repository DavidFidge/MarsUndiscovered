using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.ViewModels;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public interface IFieldOfViewTileEntityFactory
    {
        FieldOfViewTileEntity Create(Point position);
        void Release(FieldOfViewTileEntity mapTileEntity);
    }
}