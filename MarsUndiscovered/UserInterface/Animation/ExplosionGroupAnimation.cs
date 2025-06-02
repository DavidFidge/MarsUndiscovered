using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.UserInterface.ViewModels;
using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class ExplosionGroupAnimation : TileAnimation
    {
        private readonly HashSet<Point> _positions;
        private const double ExplosionAnimationSeconds = 0.1d;
        private double _elapsedRealTime;
        
        public ExplosionGroupAnimation(HashSet<Point> positions)
        {
            _positions = positions;
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            _elapsedRealTime += gameTimeService.GameTime.ElapsedRealTime.TotalSeconds;

            if (_elapsedRealTime > ExplosionAnimationSeconds)
            {
                ClearAnimationTiles(mapViewModel);
                IsComplete = true;
            }
       
            mapViewModel.AnimateAttackTile(_position, mapTileEntity => mapTileEntity.SetExplosion());
        }

        private void ClearAnimationTiles(IMapViewModel mapViewModel)
        {
            mapViewModel.ClearAnimationAttackTile(_position);
        }

        public override void Finish(IMapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}