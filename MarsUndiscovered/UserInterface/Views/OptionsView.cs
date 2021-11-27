using System.Threading;
using System.Threading.Tasks;

using Augmented.Messages;
using Augmented.UserInterface.Data;
using Augmented.UserInterface.ViewModels;

using DavidFidge.Monogame.Core.View;
using DavidFidge.Monogame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;

namespace Augmented.UserInterface.Views
{
    public class OptionsView : BaseView<OptionsViewModel, OptionsData>,
        IRequestHandler<OpenVideoOptionsRequest>,
        IRequestHandler<CloseVideoOptionsRequest>
    {
        private readonly VideoOptionsView _videoOptionsView;
        private Panel _optionsMenuPanel;

        public OptionsView(
            OptionsViewModel optionsViewModel,
            VideoOptionsView videoOptionsView)
        : base(optionsViewModel)
        {
            _videoOptionsView = videoOptionsView;
        }

        protected override void InitializeInternal()
        {
            _optionsMenuPanel = new Panel(new Vector2(500, 400));

            RootPanel.AddChild(_optionsMenuPanel);

            var headingLabel = new Label(Data.Heading, Anchor.AutoCenter)
                .H4Heading();

            _optionsMenuPanel.AddChild(headingLabel);

            var line = new HorizontalLine(Anchor.AutoCenter);

            _optionsMenuPanel.AddChild(line);

            SetupVideoOptionsItem();

            new Button("Game Options (not implemented yet")
                .SendOnClick<OpenGameOptionsRequest>(Mediator)
                .AddTo(_optionsMenuPanel);

            new Button("Back")
                .SendOnClick<CloseOptionsViewRequest>(Mediator)
                .AddTo(_optionsMenuPanel);
        }

        private void SetupVideoOptionsItem()
        {
            new Button("Video Options")
                .SendOnClick<OpenVideoOptionsRequest>(Mediator)
                .AddTo(_optionsMenuPanel);

            _videoOptionsView.Initialize();
            
            RootPanel.AddChild(_videoOptionsView.RootPanel);
        }

        public Task<Unit> Handle(OpenVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _videoOptionsView.Show();

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _videoOptionsView.Hide();

            return Unit.Task;
        }
    }
}