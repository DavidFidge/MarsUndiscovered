using System;
using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;

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
            _titleMenuPanel = new Panel();
            _titleMenuPanel.AdjustHeightAutomatically = true;
            _titleMenuPanel.Anchor = Anchor.BottomRight;
            _titleMenuPanel.Offset = new Vector2(100f, 100f);
            _titleMenuPanel.Opacity = 200;

            RootPanel.AddChild(_titleMenuPanel);

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
            _titleMenuPanel.Visible = false;

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseOptionsViewRequest request, CancellationToken cancellationToken)
        {
            _optionsView.Hide();
            _titleMenuPanel.Visible = true;

            return Unit.Task;
        }
    }
}