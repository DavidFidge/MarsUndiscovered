using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapEntity : Entity, IDrawable, IDisposable
    {
        private TexturedQuadTemplate _mapQuad;
        private float _mapWidth;
        private float _mapHeight;
        private float _tileWidth;
        private float _tileHeight;

        public bool IsVisible { get; set; } = true;

        public float MapWidth => _mapWidth;
        public float HalfMapWidth => _mapWidth / 2f;
        public float MapHeight => _mapHeight;
        public float HalfMapHeight => _mapHeight / 2f;

        public MapEntity(IGameProvider gameProvider)
        {
            _mapQuad = new TexturedQuadTemplate(gameProvider);
            _mapQuad.AlphaEnabled = false;
        }
        
        public void Initialize(float tileWidth, float tileHeight)
        {
            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            _mapQuad.Draw(view, projection, world);
        }

        public void SetMapTexture(Texture2D texture)
        {
            _mapQuad.Texture = texture;
        }

        public void LoadContent(int mapWidth, int mapHeight)
        {
            _mapWidth = mapWidth * _tileWidth;
            _mapHeight = mapHeight * _tileHeight;
            
            var translation = new Vector3(0, 0, -15f);

            Transform.ChangeTranslation(translation);        
            
            _mapQuad.LoadContent(_mapWidth, _mapHeight, null);
        }

        public void Dispose()
        {
            _mapQuad?.Dispose();
        }
    }
}