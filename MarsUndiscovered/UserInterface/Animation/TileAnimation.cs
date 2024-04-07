using FrigidRogue.MonoGame.Core.Interfaces.Services;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class TileAnimation
    {
        public abstract void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel);
        public bool IsComplete { get; protected set; }
        public abstract void Finish(IMapViewModel mapViewModel);
    }
}