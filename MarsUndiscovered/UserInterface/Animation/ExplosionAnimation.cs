using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.UserInterface.ViewModels;
using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class ExplosionAnimation : TileAnimation
    {
        private readonly Point _position;
        private const double ExplosionAnimationSeconds = 0.1d;
        private double _elapsedRealTime;
        
        public ExplosionAnimation(Point position)
        {
            _position = position;
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            _elapsedRealTime += gameTimeService.GameTime.ElapsedRealTime.TotalSeconds;

            if (_elapsedRealTime > ExplosionAnimationSeconds)
            {
                ClearAnimationTiles(mapViewModel);
                IsComplete = true;
            }
       
            mapViewModel.AnimateAttackTile(_position, mapTileEntity => mapTileEntity.SetExplosion(1f));
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