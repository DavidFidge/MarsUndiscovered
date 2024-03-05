using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.UserInterface.ViewModels;
using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class LineAttackAnimation : TileAnimation
    {
        // First point is always the player's position which will have no animation
        private readonly List<Point> _path;
        private const double LineAttackAnimationSeconds = 0.1d;
        private double _elapsedRealTime;
        private bool _firstLoop = true;
        
        public LineAttackAnimation(List<Point> path)
        {
            _path = path;
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            if (_path.Count <= 1)
            {
                IsComplete = true;
                return;
            }
            if (_firstLoop)
            {
                _firstLoop = false;

                var direction = Direction.GetDirection(_path.First(), _path.Skip(1).First());
                
                foreach (var point in _path.Skip(1))
                {
                    mapViewModel.AnimateTile(point, (mapTileEntity) => mapTileEntity.SetLineAttack(direction));
                }
            }
            else
            {
                _elapsedRealTime += gameTimeService.GameTime.ElapsedRealTime.TotalSeconds;

                if (_elapsedRealTime > LineAttackAnimationSeconds)
                {
                    ClearAnimationTiles(mapViewModel);
                    IsComplete = true;
                }
            }
        }

        private void ClearAnimationTiles(IMapViewModel mapViewModel)
        {
            mapViewModel.ClearAnimationTiles(_path.Skip(1));
        }

        public override void Finish(IMapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}