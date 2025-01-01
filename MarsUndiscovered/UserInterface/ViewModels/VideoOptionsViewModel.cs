using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class VideoOptionsViewModel : BaseViewModel<VideoOptionsData>,
        IRequestHandler<SetDisplayModeRequest>,
        IRequestHandler<SaveVideoOptionsRequest>,
        IRequestHandler<SetRenderResolutionRequest>
    {
        private readonly IGameOptionsStore _gameOptionsStore;
        private readonly IUserInterface _userInterface;
        private readonly IGameCamera _gameCamera;

        public VideoOptionsViewModel(
            IGameOptionsStore gameOptionsStore,
            IUserInterface userInterface,
            IGameCamera gameCamera)
        {
            _gameOptionsStore = gameOptionsStore;
            _userInterface = userInterface;
            _gameCamera = gameCamera;
        }

        public override void Initialize()
        {
            Data = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State ?? new VideoOptionsData();

            Data.DisplayDimensions = Game
                .CustomGraphicsDeviceManager
                .GetSupportedDisplayModes()
                .OrderBy(dm => dm.Height)
                .ToList();

            if (!Data.DisplayDimensions.Any(dm => Equals(dm, Data.SelectedDisplayDimension)))
            {
                Data.IsVerticalSync = true;
                Data.IsFullScreen = true;
                Data.IsBorderlessWindowed = true;

                Data.SelectedDisplayDimension = Data.DisplayDimensions
                    .Where(dm => dm.Width == Game.CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width)
                    .Where(dm => dm.Height == Game.CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height)
                    .FirstOrDefault();
            }

            Data.RenderResolutions = RenderResolution.RenderResolutions;

            if (Data.SelectedRenderResolution == null)
                Data.SelectedRenderResolution = RenderResolution.Default;
        }

        public void Handle(SetDisplayModeRequest request)
        {
            // Geonbit UI doesn't appear to detect changes in resolution at full screen properly which makes the text all fuzzy.
            // Get around this by changing to windowed mode, setting the resolution,
            // then going back to full screen mode.
            Data.SelectedDisplayDimension = request.DisplayDimension;

            if (Data.IsFullScreen && !Data.IsBorderlessWindowed)
            {
                Data.IsFullScreen = false;
                SetGraphicsDisplayMode();
                Data.IsFullScreen = true;
            }

            SetGraphicsDisplayMode();
        }

        public void Handle(SetRenderResolutionRequest request)
        {
            Data.SelectedRenderResolution = request.RenderResolution;

            _userInterface.RenderResolution = Data.SelectedRenderResolution;
            _gameCamera.RenderResolution = Data.SelectedRenderResolution;
        }

        private void SetGraphicsDisplayMode()
        {
            var displaySettings = new DisplaySettings(
                Data.SelectedDisplayDimension,
                Data.IsFullScreen,
                Data.IsVerticalSync,
                Data.IsBorderlessWindowed
            );

            GameProvider
                .Game
                .CustomGraphicsDeviceManager
                .SetDisplayMode(displaySettings);
        }

        public void Handle(SaveVideoOptionsRequest request)
        {
            _gameOptionsStore.SaveToStore(new Memento<VideoOptionsData>(Data));
        }

        public void HandleIsFullScreen(InterfaceRequest<VideoOptionsData> request)
        {
            SetGraphicsDisplayMode();
        }

        public void HandleIsVerticalSync(InterfaceRequest<VideoOptionsData> request)
        {
            SetGraphicsDisplayMode();
        }

        public void HandleIsBorderlessWindowed(InterfaceRequest<VideoOptionsData> request)
        {
            SetGraphicsDisplayMode();
        }
    }
}