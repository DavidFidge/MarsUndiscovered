using MarsUndiscovered.Game.DependencyInjection;
using MarsUndiscovered.UserInterface.ViewModels;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Components
{
    public class MapTileEntityFactory : IMapTileEntityFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MapTileEntityFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MapTileEntity Create(Point position)
        {
            return _serviceProvider.CreateWithInjectedProperties<MapTileEntity>(position);
        }

        public void Release(MapTileEntity mapTileEntity)
        {
        }
    }
}
