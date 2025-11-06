using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Tests.Components
{
    public class TestStory : IStory
    {
        public void Initialize(IGameWorld gameWorld)
        {
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            gameWorld.Story = new TestStory();
        }

        public void NewGame(IGameWorld gameWorld)
        {
        }

        public void NextTurn()
        {
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
        }
    }
}
