using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using GoRogue.Pathing;
using MarsUndiscovered.UserInterface.ViewModels;

using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class LaserAnimation : TileAnimation
    {
        private const float LaserTilePropagationTime = 0.02f;
        private const int LaserStreakLength = 7;
        private Tween<Vector2> _tween;
        private Tween<Vector2> _finalTween;
        private Queue<Point> _laserStreak = new Queue<Point>(LaserStreakLength);
        private float[] _laserOpacity = { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f };
        private Point _finalTweenCurrentPoint = Point.None;

        public LaserAnimation(Path path)
        {
            var tweener = new Tweener();

            var laserSpeed = laser.Path.Count * LaserTilePropagationTime;
            _tween = tweener.TweenTo(laser, from => from.From, laser.To, laserSpeed);

            var finalLine = laser.Path.TakeLast(LaserStreakLength).ToList();
            var finalLaserSpeed = finalLine.Count * LaserTilePropagationTime;
            var finalLaser = new Laser(finalLine);

            _finalTweenCurrentPoint = finalLine.First();
            _finalTween = tweener.TweenTo(finalLaser, from => from.From, finalLaser.To, finalLaserSpeed);
        }

        public override void Update(IGameTimeService gameTimeService, MapViewModel mapViewModel)
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

                    if (_laserStreak.Any())
                    {
                        _laserStreak.Dequeue();
                        AnimateLaserStreakTiles(mapViewModel);
                    }
                }

                IsComplete = _finalTween.IsComplete;
            }
            else
            {
                // Animates the tail appearing then traveling to target
                _tween.Update((float)gameTimeService.GameTime.ElapsedRealTime.TotalSeconds);

                var newCurrentPoint = _tween.Member.Value.FromVectorRounded();

                if (_laserStreak.IsEmpty() || newCurrentPoint != _laserStreak.Last())
                {
                    ClearAnimationTiles(mapViewModel);

                    if (_laserStreak.Count == LaserStreakLength)
                    {
                        _laserStreak.Dequeue();
                    }

                    _laserStreak.Enqueue(newCurrentPoint);

                    AnimateLaserStreakTiles(mapViewModel);
                }
            }
        }

        private void AnimateLaserStreakTiles(MapViewModel mapViewModel)
        {
            var index = 0;

            foreach (var point in _laserStreak.Reverse())
            {
                mapViewModel.AnimateTile(point, (mapTileEntity) => mapTileEntity.SetLaser(_laserOpacity[index]));
                index++;
            }
        }

        private void ClearAnimationTiles(MapViewModel mapViewModel)
        {
            mapViewModel.ClearAnimationTiles(_laserStreak);
        }

        public override void Finish(MapViewModel mapViewModel)
        {
            ClearAnimationTiles(mapViewModel);
        }
    }
}