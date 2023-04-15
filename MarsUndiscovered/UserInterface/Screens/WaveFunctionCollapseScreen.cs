using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.View;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;

using MediatR;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class WaveFunctionCollapseScreen : Screen
    {
        public WaveFunctionCollapseScreen(WaveFunctionCollapseView waveFunctionCollapseView) : base(waveFunctionCollapseView)
        {
        }
    }
}