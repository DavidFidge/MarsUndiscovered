using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class WorldBuilderView : BaseGameView<WorldBuilderViewModel, WorldBuilderData>,
        IRequestHandler<BuildWorldRequest>,
        IRequestHandler<OpenWorldBuilderOptionsRequest>,
        IRequestHandler<CloseWorldBuilderOptionsRequest>
    {
        private readonly WorldBuilderOptionsView _worldBuilderOptionsView;

        public WorldBuilderView(
            WorldBuilderViewModel worldBuilderViewModel,
            WorldBuilderOptionsView worldBuilderOptionsView,
            IGameCamera gameCamera
        )
            : base(gameCamera, worldBuilderViewModel)
        {
            _worldBuilderOptionsView = worldBuilderOptionsView;
        }

        protected override void InitializeInternal()
        {
            CreateLayoutPanels();
            SetupWorldBuilderOptionsButton(LeftPanel);
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
            SetupChildPanel(_worldBuilderOptionsView);
        }

        private void SetupWorldBuilderOptionsButton(Panel leftPanel)
        {
            var menuButton = new Button(
                    "-",
                    ButtonSkin.Default,
                    Anchor.AutoInline,
                    new Vector2(50, 50)
                )
                .SendOnClick<OpenWorldBuilderOptionsRequest>(Mediator)
                .NoPadding();

            leftPanel.AddChild(menuButton);
        }
        
        public void LoadWorldBuilder()
        {
            ResetViews();
            _viewModel.BuildWorld();
        }

        public Task<Unit> Handle(BuildWorldRequest request, CancellationToken cancellationToken)
        {
            ResetViews();
            _viewModel.BuildWorld();

            return Unit.Task;
        }

        public Task<Unit> Handle(OpenWorldBuilderOptionsRequest request, CancellationToken cancellationToken)
        {
            _worldBuilderOptionsView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseWorldBuilderOptionsRequest request, CancellationToken cancellationToken)
        {
            _worldBuilderOptionsView.Hide();
            return Unit.Task;
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();
        }
    }
}