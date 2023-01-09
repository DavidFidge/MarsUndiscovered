using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapEntity : Entity, IDrawable, ILoadContent
    {
        private TexturedQuadTemplate _mapQuad;
        private float _mapWidth;
        private float _mapHeight;

        public MapEntity(IGameProvider gameProvider)
        {
            _mapQuad = new TexturedQuadTemplate(gameProvider);
            _mapQuad.AlphaEnabled = false;
        }
        
        public void Initialize(int mapUnitWidth, int mapUnitHeight, float tileWidth, float tileHeight)
        {
            _mapWidth = mapUnitWidth * tileWidth;
            _mapHeight = mapUnitHeight * tileHeight;
            
            var halfTileHeight = tileHeight / 2f;

            var translation = new Vector3(-_mapWidth / 2f, (_mapHeight / 2f) - halfTileHeight, (-_mapHeight / 2f));

            var offsetFromCentre = new Vector3(tileWidth * 10f, tileHeight * -1.7f, -3.1f);

            translation += offsetFromCentre;

            Transform.ChangeTranslation(translation);
        }

        public bool IsVisible { get; set; } = true;
        
        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            _mapQuad.Draw(view, projection, world);
        }

        public void SetMapTexture(Texture2D texture)
        {
            _mapQuad.Texture = texture;
        }

        public void LoadContent()
        {
            _mapQuad.LoadContent(_mapWidth, _mapHeight, null);
        }
    }
}