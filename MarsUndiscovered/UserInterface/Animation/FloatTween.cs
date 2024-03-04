using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MonoGame.Extended.Tweening;

namespace MarsUndiscovered.UserInterface.Animation
{
    public class FloatTween
    {
        private Tween<float> _tween;
        private float _value;
        public float Value => _value;

        public bool IsComplete => _tween.IsComplete;
        
        public FloatTween(float start, float end, float duration)
        {
            _value = start;
            var tweener = new Tweener();
            _tween = tweener.TweenTo(this, l => l._value, end, duration);
            _tween.Easing(EasingFunctions.Linear);
        }
        
        public void Update(IGameTimeService gameTimeService)
        {
            _tween.Update((float)gameTimeService.GameTime.ElapsedRealTime.TotalSeconds);
        }
    }
}