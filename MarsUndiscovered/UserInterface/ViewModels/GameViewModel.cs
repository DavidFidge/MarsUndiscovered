using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseViewModel<GameData>
    {
        public IGameWorld GameWorld { get; set; }

        public GameViewModel()
        {
        }
    }
}