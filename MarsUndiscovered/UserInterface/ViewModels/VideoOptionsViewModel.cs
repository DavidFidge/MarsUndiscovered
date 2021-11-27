using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Augmented.Messages;
using Augmented.UserInterface.Data;

using DavidFidge.MonoGame.Core.Interfaces.Components;
using DavidFidge.MonoGame.Core.Interfaces.Services;
using DavidFidge.MonoGame.Core.Services;
using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

namespace Augmented.UserInterface.ViewModels
{
    public class VideoOptionsViewModel : BaseViewModel<VideoOptionsData>,
        IRequestHandler<SetDisplayModeRequest>,
        IRequestHandler<SaveVideoOptionsRequest>,
        IRequestHandler<VideoOptionsFullScreenToggle>,
        IRequestHandler<VideoOptionsVerticalSyncToggle>
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

            if (!Data.DisplayModes.Any(dm => Equals(dm, Data.SelectedDisplayMode)))
            {
                Data.IsVerticalSync = true;
                Data.IsFullScreen = false;

                Data.SelectedDisplayMode = Data.DisplayModes
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
            Data.SelectedDisplayMode = request.DisplayMode;

            if (Data.IsFullScreen)
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
            GameProvider.Game
                .CustomGraphicsDeviceManager
                .SetDisplayMode(
                    Data.SelectedDisplayMode,
                    Data.IsFullScreen
                );

            GameProvider.Game.CustomGraphicsDeviceManager.ApplyChanges();
        }

        public Task<Unit> Handle(SaveVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _gameOptionsStore.SaveToStore(new Memento<VideoOptionsData>(Data));

            return Unit.Task;
        }

        public Task<Unit> Handle(VideoOptionsFullScreenToggle request, CancellationToken cancellationToken)
        {
            Data.IsFullScreen = request.IsChecked;
            SetGraphicsDisplayMode();

            return Unit.Task;
        }

        public Task<Unit> Handle(VideoOptionsVerticalSyncToggle request, CancellationToken cancellationToken)
        {
            Data.IsVerticalSync = request.IsChecked;

            GameProvider.Game
                .CustomGraphicsDeviceManager
                .IsVerticalSync = request.IsChecked;

            GameProvider.Game.CustomGraphicsDeviceManager.ApplyChanges();

            return Unit.Task;
        }
    }
}