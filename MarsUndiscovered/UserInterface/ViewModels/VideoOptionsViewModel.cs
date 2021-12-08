using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Interfaces;

using MediatR;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class VideoOptionsViewModel : BaseViewModel<VideoOptionsData>,
        IRequestHandler<SetDisplayModeRequest>,
        IRequestHandler<SaveVideoOptionsRequest>,
        IRequestHandler<SetRenderResolutionRequest>
    {
        private readonly IGameOptionsStore _gameOptionsStore;
        private readonly IUserInterface _userInterface;
        public IGameProvider GameProvider { get; set; }

        public VideoOptionsViewModel(
            IGameOptionsStore gameOptionsStore,
            IUserInterface userInterface)
        {
            _gameOptionsStore = gameOptionsStore;
            _userInterface = userInterface;
        }

        public override void Initialize()
        {
            Data = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State ?? new VideoOptionsData();

            Data.DisplayDimensions = GameProvider.Game
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
                    .Where(dm => dm.Width == GameProvider.Game.CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width)
                    .Where(dm => dm.Height == GameProvider.Game.CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height)
                    .FirstOrDefault();
            }

            Data.RenderResolutions = RenderResolution.RenderResolutions;

            if (Data.SelectedRenderResolution == null)
                Data.SelectedRenderResolution = RenderResolution.Default;
        }

        public Task<Unit> Handle(
            SetDisplayModeRequest request,
            CancellationToken cancellationToken
        )
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

            return Unit.Task;
        }

        public Task<Unit> Handle(SetRenderResolutionRequest request, CancellationToken cancellationToken)
        {
            Data.SelectedRenderResolution = request.RenderResolution;

            _userInterface.RenderResolution = Data.SelectedRenderResolution;

            return Unit.Task;
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

        public Task<Unit> Handle(SaveVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _gameOptionsStore.SaveToStore(new Memento<VideoOptionsData>(Data));

            return Unit.Task;
        }

        public Task<Unit> HandleIsFullScreen(InterfaceRequest<VideoOptionsData> request, CancellationToken cancellationToken)
        {
            SetGraphicsDisplayMode();

            return Unit.Task;
        }

        public Task<Unit> HandleIsVerticalSync(InterfaceRequest<VideoOptionsData> request, CancellationToken cancellationToken)
        {
            SetGraphicsDisplayMode();

            return Unit.Task;
        }

        public Task<Unit> HandleIsBorderlessWindowed(InterfaceRequest<VideoOptionsData> request, CancellationToken cancellationToken)
        {
            SetGraphicsDisplayMode();

            return Unit.Task;
        }
    }
}