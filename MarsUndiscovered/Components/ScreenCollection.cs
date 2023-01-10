using FrigidRogue.MonoGame.Core.View.Interfaces;

using MarsUndiscovered.UserInterface.Screens;

namespace MarsUndiscovered.Components
{
    public class ScreenCollection : List<IScreen>
    {
        private readonly IScreen[] _screens;

        public ScreenCollection(IScreen[] screens)
        {
            _screens = screens;
        }

        public IScreen StartupScreen => _screens.Single(s => s is TitleScreen);

        public void Initialize()
        {
            foreach (var screen in _screens)
            {
                screen.Initialize();
            }
        }
    }
}
