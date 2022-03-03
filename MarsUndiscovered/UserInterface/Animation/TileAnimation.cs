using FrigidRogue.MonoGame.Core.Interfaces.Services;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class TileAnimation
    {
        public abstract void Update(IGameTimeService gameTimeService, MapViewModel mapViewModel);
        public bool IsComplete { get; protected set; }
        public abstract void Finish(MapViewModel mapViewModel);
    }
}