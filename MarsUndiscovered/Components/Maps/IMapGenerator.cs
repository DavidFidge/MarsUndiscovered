using GoRogue.GameFramework;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMapGenerator
    {
        ArrayView<IGameObject> CreateOutdoorWallsFloors();
        Map CreateMap(WallCollection walls, FloorCollection floors);
    }
}