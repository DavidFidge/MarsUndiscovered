using MarsUndiscovered.Game.DependencyInjection;
using MarsUndiscovered.UserInterface.ViewModels;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Components
{
    public class FieldOfViewTileEntityFactory : IFieldOfViewTileEntityFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FieldOfViewTileEntityFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public FieldOfViewTileEntity Create(Point position)
        {
            return _serviceProvider.CreateWithInjectedProperties<FieldOfViewTileEntity>(position);
        }

        public void Release(FieldOfViewTileEntity mapTileEntity)
        {
        }
    }
}
