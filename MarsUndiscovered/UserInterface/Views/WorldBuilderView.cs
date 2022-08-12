using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using MediatR;

namespace MarsUndiscovered.UserInterface.Views
{
    public class WorldBuilderView : BaseGameCoreView<WorldBuilderViewModel, WorldBuilderData>,
        IRequestHandler<BuildWorldRequest>,
        IRequestHandler<OpenWorldBuilderOptionsRequest>,
        IRequestHandler<CloseWorldBuilderOptionsRequest>,
        IRequestHandler<NextWorldBuilderStepRequest>,
        IRequestHandler<PreviousWorldBuilderStepRequest>
    {
        private readonly WorldBuilderOptionsView _worldBuilderOptionsView;
        
        protected Panel LeftPanel;

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
            SetupChildPanel(_worldBuilderOptionsView);
        }

        protected void CreateLayoutPanels()
        {
            LeftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(0.19f)
                .NoSkin()
                .NoPadding()
                .Height(0.999f);
            
            new Button("Menu")
                .SendOnClick<OpenWorldBuilderOptionsRequest>(Mediator)
                .AddTo(LeftPanel);

            new Button("Build New World")
                .SendOnClick<BuildWorldRequest>(Mediator)
                .AddTo(LeftPanel);
     
            new Button("Next Step")
                .SendOnClick<NextWorldBuilderStepRequest>(Mediator)
                .AddTo(LeftPanel);
            
            new Button("Previous Step")
                .SendOnClick<PreviousWorldBuilderStepRequest>(Mediator)
                .AddTo(LeftPanel);
            
            RootPanel.AddChild(LeftPanel);
        }

        public void LoadWorldBuilder()
        {
            _viewModel.BuildWorld();
        }

        public Task<Unit> Handle(BuildWorldRequest request, CancellationToken cancellationToken)
        {
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

        public Task<Unit> Handle(NextWorldBuilderStepRequest request, CancellationToken cancellationToken)
        {
            _viewModel.NextStep();
            return Unit.Task;
        }

        public Task<Unit> Handle(PreviousWorldBuilderStepRequest request, CancellationToken cancellationToken)
        {
            _viewModel.PreviousStep();
            return Unit.Task;
        }
    }
}