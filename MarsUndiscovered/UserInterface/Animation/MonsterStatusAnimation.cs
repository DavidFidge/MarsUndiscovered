using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.UserInterface.ViewModels;
using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class MonsterStatusAnimation : TileAnimation
    {
        // First point is always the player's position which will have no animation
        private const double BlinkSeconds = 0.25d;
        private double _elapsedRealTime;
        private MonsterStatus _monsterStatus;
        private Point _point;
        private double _totalAnimationTime = 2d;

        public MonsterStatusAnimation(MonsterStatus monsterStatus, Point point)
        {
            _monsterStatus = monsterStatus;
            _point = point;
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            var delta = gameTimeService.GameTime.ElapsedRealTime.TotalSeconds;
            _elapsedRealTime += delta;

            // Create a blink cycle: on for BlinkSeconds, off for BlinkSeconds
            var cycleLength = BlinkSeconds * 2;
            var prevCyclePos = ((_elapsedRealTime - delta) % cycleLength + cycleLength) % cycleLength;
            var cyclePos = (_elapsedRealTime % cycleLength + cycleLength) % cycleLength;

            if (cyclePos < BlinkSeconds)
            {
                mapViewModel.AnimateAttackTile(_point, mapTileEntity => mapTileEntity.SetMonsterStateOverlay(_monsterStatus.MonsterState));
            }
            else
            {
                // In the "off" phase: ensure the animated tile is cleared so it appears to blink
                ClearAnimationTiles(mapViewModel);
            }

            if (_elapsedRealTime >= _totalAnimationTime)
            {
                ClearAnimationTiles(mapViewModel);
                IsComplete = true;
            }
        }

        public void ClearAnimationTiles(IMapViewModel mapViewModel)
        {
            mapViewModel.ClearAnimationAttackTile(_point);
        }

        public override void Finish(IMapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}