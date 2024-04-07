using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MonoGame.Extended.Tweening;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class FloatTween
    {
        private Tween<float> _tween;
        private readonly Tweener _tweener;
        public float Value { get; set; }

        public bool IsComplete => _tween.IsComplete;
        
        public FloatTween(float start, float end, float duration)
        {
            Value = start;
            _tweener = new Tweener();
            _tween = _tweener.TweenTo(this, l => l.Value, end, duration);
            _tween.Easing(EasingFunctions.Linear);
        }
        
        public void Update(IGameTimeService gameTimeService)
        {
            _tweener.Update((float)gameTimeService.GameTime.ElapsedRealTime.TotalSeconds);
        }
    }
}