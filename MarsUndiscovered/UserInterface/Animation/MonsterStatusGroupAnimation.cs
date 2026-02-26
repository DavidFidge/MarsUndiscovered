using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class MonsterStatusGroupAnimation : TileAnimation
    {
        private List<MonsterStatusAnimation> _monsterStatusAnimations;

        public MonsterStatusGroupAnimation(List<MonsterStatusAnimation> monsterStatusAnimations)
        {
            _monsterStatusAnimations = monsterStatusAnimations;
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            foreach (var item in _monsterStatusAnimations)
            {
                item.Update(gameTimeService, mapViewModel);
            }

            this.IsComplete = _monsterStatusAnimations.All(x => x.IsComplete);
        }

        private void ClearAnimationTiles(IMapViewModel mapViewModel)
        {
            foreach (var item in _monsterStatusAnimations)
            {
                item.ClearAnimationTiles(mapViewModel);
            }
        }

        public override void Finish(IMapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}