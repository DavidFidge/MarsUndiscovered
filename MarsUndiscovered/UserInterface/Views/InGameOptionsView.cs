﻿using System.Threading;
using System.Threading.Tasks;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MediatR;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InGameOptionsView : BaseMarsUndiscoveredView<InGameOptionsViewModel, InGameOptionsData>,
        IRequestHandler<OpenInGameVideoOptionsRequest>,
        IRequestHandler<CloseInGameVideoOptionsRequest>,
        IRequestHandler<OpenInGameGameOptionsRequest>,
        IRequestHandler<CloseInGameGameOptionsRequest>,
        IRequestHandler<OpenSaveGameViewRequest>,
        IRequestHandler<CloseSaveGameViewRequest>
    {
        private readonly VideoOptionsView _videoOptionsView;
        private readonly GameOptionsView _gameOptionsView;
        private readonly SaveGameView _saveGameView;
        private Panel _inGameOptionsMenuPanel;

        public InGameOptionsView(
            InGameOptionsViewModel inGameOptionsViewModel,
            VideoOptionsView videoOptionsView,
            GameOptionsView gameOptionsView,
            SaveGameView saveGameView
        ) : base(inGameOptionsViewModel)
        {
            _gameOptionsView = gameOptionsView;
            _videoOptionsView = videoOptionsView;
            _saveGameView = saveGameView;
        }

        protected override void InitializeInternal()
        {
            _inGameOptionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

            RootPanel.AddChild(_inGameOptionsMenuPanel);

            SetupChildPanelWithButton<OpenSaveGameViewRequest, SaveGameViewModel, SaveGameData>(_inGameOptionsMenuPanel, "Save Game", _saveGameView);

            SetupSharedChildPanelWithButton<OpenInGameVideoOptionsRequest, VideoOptionsViewModel, VideoOptionsData>(_inGameOptionsMenuPanel, "Video Options", _videoOptionsView);

            SetupSharedChildPanelWithButton<OpenInGameGameOptionsRequest, GameOptionsViewModel, GameOptionsData>(_inGameOptionsMenuPanel, "Game Options", _gameOptionsView);
            
            new Button("Exit Game")
                .SendOnClick<CloseInGameOptionsRequest, QuitToTitleRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);

            new Button("Back to Game")
                .SendOnClick<CloseInGameOptionsRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);
        }

        public Task<Unit> Handle(OpenSaveGameViewRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_saveGameView, _inGameOptionsMenuPanel);
        }

        public Task<Unit> Handle(CloseSaveGameViewRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_saveGameView, _inGameOptionsMenuPanel);
        }

        public Task<Unit> Handle(OpenInGameVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            return ShowChildViewWithRootSwap(_videoOptionsView, _inGameOptionsMenuPanel);
        }

        public Task<Unit> Handle(CloseInGameVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_videoOptionsView, _inGameOptionsMenuPanel);
        }

        public Task<Unit> Handle(OpenInGameGameOptionsRequest request, CancellationToken cancellationToken)
        {
            return ShowChildViewWithRootSwap(_gameOptionsView, _inGameOptionsMenuPanel);
        }

        public Task<Unit> Handle(CloseInGameGameOptionsRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_gameOptionsView, _inGameOptionsMenuPanel);
        }
    }
}