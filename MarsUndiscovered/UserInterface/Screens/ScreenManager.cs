using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;

using FrigidRogue.Monogame.Core.View;

using MediatR;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class ScreenManager : IScreenManager,
        IRequestHandler<NewGameRequest>,
        IRequestHandler<ExitCurrentGameRequest>
    {
        private readonly TitleScreen _titleScreen;
        private readonly GameScreen _gameScreen;

        private Screen _activeScreen;

        public ScreenManager(
            TitleScreen titleScreen,
            GameScreen gameScreen
        )
        {
            _titleScreen = titleScreen;
            _gameScreen = gameScreen;
        }

        public void Initialize()
        {
            _titleScreen.Initialize();
            _gameScreen.Initialize();
        }

        public void Draw()
        {
            _activeScreen?.Draw();
        }

        private void ShowScreen(Screen screen)
        {
            _activeScreen?.Hide();

            screen.Show();

            _activeScreen = screen;
        }

        public void Update()
        {
            if (_activeScreen == null)
            {
                // Quickstart to ease debugging
                NewGame();
                // ShowScreen(_titleScreen);
            }

            _activeScreen.Update();
        }
        
        public Task<Unit> Handle(NewGameRequest request, CancellationToken cancellationToken)
        {
            NewGame();

            return Unit.Task;
        }

        private void NewGame()
        {
            ShowScreen(_gameScreen);
            _gameScreen.StartNewGame();
        }

        public Task<Unit> Handle(ExitCurrentGameRequest request, CancellationToken cancellationToken)
        {
            _gameScreen.EndGame();

            ShowScreen(_titleScreen);
            return Unit.Task;
        }
    }
}