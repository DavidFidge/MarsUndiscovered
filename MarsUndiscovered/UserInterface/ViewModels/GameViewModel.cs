using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseViewModel<GameData>
    {
        public GameViewModel(IGameWorld gameWorld)
        {
            gameWorld.Generate();
            Data = new GameData();
            Data.WallsFloors = (ArrayView<bool>)gameWorld.WallsFloors.Clone();
        }
    }
}