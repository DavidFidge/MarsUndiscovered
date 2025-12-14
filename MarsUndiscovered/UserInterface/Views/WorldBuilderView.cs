using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

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

        public Label StepLabel { get; private set; }
        public Label FailedLabel { get; private set; }

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
            base.InitializeInternal();

            CreateLayoutPanels();
            SetupChildPanel(_worldBuilderOptionsView);
        }

        protected void CreateLayoutPanels()
        {
            LeftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(UiConstants.LeftPanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(1f);
            
            new Button("Menu")
                .SendOnClick<OpenWorldBuilderOptionsRequest>(Mediator)
                .AddTo(LeftPanel);

            new Button("Build New Outdoor World")
                .SendOnClick(Mediator, new BuildWorldRequest { WorldGenerationTypeParams = new WorldGenerationTypeParams(MapType.Outdoor)})
                .AddTo(LeftPanel);
            
            new Button("Build New Mine World")
                .SendOnClick(Mediator, new BuildWorldRequest { WorldGenerationTypeParams = new WorldGenerationTypeParams(MapType.Mine)})
                .AddTo(LeftPanel);

            new Button("Build New Mining Facility")
                .SendOnClick(Mediator, new BuildWorldRequest { WorldGenerationTypeParams = new WorldGenerationTypeParams(MapType.MiningFacility)})
                .AddTo(LeftPanel);
            
            new Button("Build New Prefab Map")
                .SendOnClick(Mediator, new BuildWorldRequest { WorldGenerationTypeParams = new WorldGenerationTypeParams(MapType.Prefab)})
                .AddTo(LeftPanel);
            
            new Button("Next Step")
                .SendOnClick<NextWorldBuilderStepRequest>(Mediator)
                .AddTo(LeftPanel);
            
            new Button("Previous Step")
                .SendOnClick<PreviousWorldBuilderStepRequest>(Mediator)
                .AddTo(LeftPanel);

            StepLabel = new Label()
                .AddTo(LeftPanel);

            FailedLabel = new Label("Map generation failed")
                .Hidden()
                .AddTo(LeftPanel);
            
            RootPanel.AddChild(LeftPanel);
        }

        public void LoadWorldBuilder()
        {
            _viewModel.BuildWorld(new WorldGenerationTypeParams(MapType.Outdoor));
        }

        public void Handle(BuildWorldRequest request)
        {
            FailedLabel.Hidden();
            StepLabel.Text = String.Empty;

            _viewModel.BuildWorld(request.WorldGenerationTypeParams);
            UpdateMapRenderTarget(_viewModel.MapViewModel.Width, _viewModel.MapViewModel.Height);
        }

        public void Handle(OpenWorldBuilderOptionsRequest request)
        {
            _worldBuilderOptionsView.Show();
        }

        public void Handle(CloseWorldBuilderOptionsRequest request)
        {
            _worldBuilderOptionsView.Hide();
        }

        public void Handle(NextWorldBuilderStepRequest request)
        {
            _viewModel.NextStep();

            if (_viewModel.Failed)
                FailedLabel.Visible();

            StepLabel.Text = $"{_viewModel.CurrentStep}{(_viewModel.IsFinalStep ? "Final" : String.Empty)}";
        }

        public void Handle(PreviousWorldBuilderStepRequest request)
        {
            FailedLabel.Hidden();

            _viewModel.PreviousStep();

            StepLabel.Text = $"{_viewModel.CurrentStep}";
        }
    }
}