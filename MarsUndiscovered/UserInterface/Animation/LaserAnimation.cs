using FrigidRogue.MonoGame.Core.Interfaces.Services;
using GoRogue.Pathing;
using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class LaserAnimation : TileAnimation
    {
        private FloatTween _floatTween;
        private readonly Path _path;

        public LaserAnimation(Path path)
        {
            _floatTween = new FloatTween(0f, 1f, 1);
            _path = path;
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            _floatTween.Update(gameTimeService);
            
            ClearAnimationTiles(mapViewModel);
            AnimateLaserStreakTiles(mapViewModel);
            IsComplete = _floatTween.IsComplete;
        }

        private void AnimateLaserStreakTiles(IMapViewModel mapViewModel)
        {
            var index = 0;

            foreach (var point in _path.Steps)
            {
                mapViewModel.AnimateTile(point, (mapTileEntity) => mapTileEntity.SetLaser(_floatTween.Value));
                index++;
            }
        }

        private void ClearAnimationTiles(IMapViewModel mapViewModel)
        {
            mapViewModel.ClearAnimationTiles(_path.Steps);
        }

        public override void Finish(IMapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}