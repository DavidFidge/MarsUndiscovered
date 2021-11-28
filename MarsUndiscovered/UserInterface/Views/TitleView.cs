using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.Monogame.Core.View;
using FrigidRogue.Monogame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class TitleView : BaseView<TitleViewModel, TitleData>,
        IRequestHandler<OptionsButtonClickedRequest>,
        IRequestHandler<CloseOptionsViewRequest>
    {
        private readonly OptionsView _optionsView;

        private Panel _titleMenuPanel;

        public TitleView(
            TitleViewModel titleViewModel,
            OptionsView optionsView)
            : base(titleViewModel)
        {
            _optionsView = optionsView;
        }

        protected override void InitializeInternal()
        {
            _titleMenuPanel = new Panel(new Vector2(500, 350f));
            RootPanel.AddChild(_titleMenuPanel);

            var headingLabel = new Label(Data.Heading, Anchor.AutoCenter)
                .H4Heading();

            _titleMenuPanel.AddChild(headingLabel);

            var line = new HorizontalLine(Anchor.AutoCenter);

            _titleMenuPanel.AddChild(line);

            new Button("New Game")
                .SendOnClick<NewGameRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            SetupOptionsItem();

            new Button("Exit")
                .SendOnClick<ExitGameRequest>(Mediator)
                .AddTo(_titleMenuPanel);
        }

        private void SetupOptionsItem()
        {
            new Button("Options")
                .SendOnClick<OptionsButtonClickedRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            _optionsView.Initialize();

            RootPanel.AddChild(_optionsView.RootPanel);
        }

        public Task<Unit> Handle(OptionsButtonClickedRequest request, CancellationToken cancellationToken)
        {
            _optionsView.Show();

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseOptionsViewRequest request, CancellationToken cancellationToken)
        {
            _optionsView.Hide();

            return Unit.Task;
        }
    }
}