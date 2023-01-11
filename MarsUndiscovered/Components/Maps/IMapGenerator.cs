using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMapGenerator
    {
        MarsMap MarsMap { get; set; }
        int Steps { get; set; }
        bool IsComplete { get; set; }
        public int MapWidthMin { get; set; }
        public int MapWidthMax { get; set; }
        public int MapHeightMin { get; set; }
        public int MapHeightMax { get; set; }
        void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null);
        void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null);
    }
}