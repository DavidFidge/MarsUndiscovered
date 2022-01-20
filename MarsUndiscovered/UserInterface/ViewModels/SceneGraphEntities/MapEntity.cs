using FrigidRogue.MonoGame.Core.Components;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapEntity : Entity
    {
        public void CreateTranslation(int mapWidth, int mapHeight, float tileWidth, float tileHeight)
        {
            var translationWidth = mapWidth * tileWidth;
            var translationHeight = mapHeight * tileHeight;
            var halfTileHeight = tileHeight / 2f;

            var translation = new Vector3(-translationWidth / 2f, (translationHeight / 2f) - halfTileHeight, (-translationHeight / 2f));

            var offsetFromCentre = new Vector3(tileWidth * 10f, tileHeight * -1.7f, -3.1f);

            translation += offsetFromCentre;

            Transform.ChangeTranslation(translation);
        }
    }
}