using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseViewModel<GameData>
    {
        public IGameWorld GameWorld { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            GameWorld.Generate();
            Data.WallsFloors = (ArrayView<bool>)GameWorld.WallsFloors.Clone();
        }
    }
}