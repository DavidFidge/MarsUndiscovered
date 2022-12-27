using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered
{
    public class FpsCounter
    {
        private double _frames = 0;
        private double _updates = 0;
        private double _elapsed = 0;
        private double _last = 0;
        private double _now = 0;
        private string _msg = "";

        private readonly double _msgFrequency = 1.0f;

        public void Update(GameTime gameTime)
        {
            _now = gameTime.TotalGameTime.TotalSeconds;
            _elapsed = (double)(_now - _last);
            if (_elapsed > _msgFrequency)
            {
                _msg = " Fps: " + (_frames / _elapsed) + "\n Elapsed time: " + _elapsed +  "\n Updates: " + _updates + "\n Frames: " + _frames;
                _elapsed = 0;
                _frames = 0;
                _updates = 0;
                _last = _now;
            }
            _updates++;
        }

        public void DrawFps(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            spriteBatch.DrawString(font, _msg, fpsDisplayPosition, fpsTextColor, 0f, Vector2.Zero, new Vector2(0.25f, 0.25f), SpriteEffects.None, 0f);
            _frames++;
        }
    }
}