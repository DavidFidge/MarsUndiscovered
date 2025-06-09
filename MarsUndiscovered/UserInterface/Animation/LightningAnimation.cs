using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class LightningAnimation : TileAnimation
    {
        private const float LightningTilePropagationTime = 0.02f;
        private const int LightningStreakLength = 7;
        private Tween<Vector2> _tween;
        private Tween<Vector2> _finalTween;
        private Queue<Point> _lightningStreak = new Queue<Point>(LightningStreakLength);
        private float[] _lightningOpacity = { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f };
        private Point _finalTweenCurrentPoint = Point.None;

        public LightningAnimation(Lightning lightning)
        {
            var tweener = new Tweener();

            var lightningSpeed = lightning.Path.Count * LightningTilePropagationTime;
            _tween = tweener.TweenTo(lightning, from => from.From, lightning.To, lightningSpeed);

            var finalLine = lightning.Path.TakeLast(LightningStreakLength).ToList();
            var finalLightningSpeed = finalLine.Count * LightningTilePropagationTime;
            var finalLightning = new Lightning(finalLine);

            _finalTweenCurrentPoint = finalLine.First();
            _finalTween = tweener.TweenTo(finalLightning, from => from.From, finalLightning.To, finalLightningSpeed);
        }

        public override void Update(IGameTimeService gameTimeService, IMapViewModel mapViewModel)
        {
            if (_tween.IsComplete)
            {
                // Animates the tail disappearing gradually once it hits the target
                _finalTween.Update((float)gameTimeService.GameTime.ElapsedRealTime.TotalSeconds);

                var fromVectorRounded = _finalTween.Member.Value.FromVectorRounded();

                if (fromVectorRounded != _finalTweenCurrentPoint)
                {
                    _finalTweenCurrentPoint = fromVectorRounded;

                    ClearAnimationTiles(mapViewModel);

                    if (_lightningStreak.Any())
                    {
                        _lightningStreak.Dequeue();
                        AnimateLightningStreakTiles(mapViewModel);
                    }
                }

                IsComplete = _finalTween.IsComplete;
            }
            else
            {
                // Animates the tail appearing then traveling to target
                _tween.Update((float)gameTimeService.GameTime.ElapsedRealTime.TotalSeconds);

                var newCurrentPoint = _tween.Member.Value.FromVectorRounded();

                if (_lightningStreak.IsEmpty() || newCurrentPoint != _lightningStreak.Last())
                {
                    ClearAnimationTiles(mapViewModel);

                    if (_lightningStreak.Count == LightningStreakLength)
                    {
                        _lightningStreak.Dequeue();
                    }

                    _lightningStreak.Enqueue(newCurrentPoint);

                    AnimateLightningStreakTiles(mapViewModel);
                }
            }
        }

        private void AnimateLightningStreakTiles(IMapViewModel mapViewModel)
        {
            var index = 0;

            foreach (var point in _lightningStreak.Reverse())
            {
                mapViewModel.AnimateAttackTile(point, mapTileEntity => mapTileEntity.SetLightning(_lightningOpacity[index]));
                index++;
            }
        }

        private void ClearAnimationTiles(IMapViewModel mapViewModel)
        {
            mapViewModel.ClearAnimationAttackTiles(_lightningStreak);
        }

        public override void Finish(IMapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}