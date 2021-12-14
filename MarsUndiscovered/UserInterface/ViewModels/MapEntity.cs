using FrigidRogue.MonoGame.Core.Components;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class MapEntity : Entity
    {
        public MapEntity()
        {
            Transform.ChangeTranslation(new Vector3(-1, -1, -1));
        }
    }
}