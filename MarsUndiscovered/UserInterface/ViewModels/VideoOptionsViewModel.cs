using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class VideoOptionsViewModel : BaseViewModel<VideoOptionsData>,
        IRequestHandler<SetDisplayModeRequest>,
        IRequestHandler<SaveVideoOptionsRequest>
    {
        private readonly IGameOptionsStore _gameOptionsStore;
        public IGameProvider GameProvider { get; set; }

        public VideoOptionsViewModel(IGameOptionsStore gameOptionsStore)
        {
            _gameOptionsStore = gameOptionsStore;
        }

        public override void Initialize()
        {
            Data = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State ?? new VideoOptionsData();
            
            Data.DisplayModes = GameProvider.Game
                .CustomGraphicsDeviceManager
                .GetSupportedDisplayModes();

            if (!Data.DisplayModes.Any(dm => Equals(dm, Data.SelectedDisplayDimensions)))
            {
                Data.IsVerticalSync = true;
                Data.IsFullScreen = true;
                Data.IsBorderlessWindowed = true;

                Data.SelectedDisplayDimensions = Data.DisplayModes
                    .Where(dm => dm.Width == GameProvider.Game.CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width)
                    .Where(dm => dm.Height == GameProvider.Game.CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height)
                    .FirstOrDefault();
            }
        }

        public Task<Unit> Handle(
            SetDisplayModeRequest request,
            CancellationToken cancellationToken
        )
        {
            // Geonbit UI doesn't appear to detect changes in resolution at full screen properly which makes the text all fuzzy.
            // Get around this by changing to windowed mode, setting the resolution,
            // then going back to full screen mode.
            Data.SelectedDisplayDimensions = request.DisplayDimensions;

            if (Data.IsFullScreen && !Data.IsBorderlessWindowed)
            {
                Data.IsFullScreen = false;
                SetGraphicsDisplayMode();
                Data.IsFullScreen = true;
            }

            SetGraphicsDisplayMode();

            return Unit.Task;
        }

        private void SetGraphicsDisplayMode()
        {
            var displaySettings = new DisplaySettings(
                Data.SelectedDisplayDimensions,
                Data.IsFullScreen,
                Data.IsVerticalSync,
                Data.IsFullScreen && Data.IsBorderlessWindowed
            );

            GameProvider
                .Game
                .CustomGraphicsDeviceManager
                .SetDisplayMode(displaySettings);

            GameProvider.Game.CustomGraphicsDeviceManager.ApplyChanges();
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