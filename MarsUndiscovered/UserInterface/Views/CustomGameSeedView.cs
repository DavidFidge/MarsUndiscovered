﻿using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Views
{
    public class CustomGameSeedView : BaseMarsUndiscoveredView<CustomGameSeedViewModel, CustomGameSeedData>,
        IRequestHandler<CustomGameSeedEnterKeyRequest>
    {
        private Panel _customGameSeedPanel;
        private TextInput _customSeed;
        private Button _startGameButton;

        public CustomGameSeedView(CustomGameSeedViewModel customGameSeedViewModel)
        : base(customGameSeedViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            _customGameSeedPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

            RootPanel.AddChild(_customGameSeedPanel);

            new Label("Enter seed:")
                .AddTo(_customGameSeedPanel);

            _customSeed = new TextInput()
                .WidthOfButton(TextInput.DefaultStyle.GetStyleProperty("DefaultSize").asVector.Y)
                .AddTo(_customGameSeedPanel);

            _customSeed.Validators.Add(new ULongNumbersOnly(0, ulong.MaxValue));
            _customSeed.Validators.Add(new SlugValidator(false));

            _startGameButton = new Button("Start Game")
                .SendOnClick<CancelCustomGameSeedRequest, NewGameRequest>(
                    request => { }, // Cancel is sent first so that the popup gets closed
                    request =>
                    {
                        if (_customSeed.Value.Length > 0)
                            request.Seed = ulong.Parse(_customSeed.Value);

                    }, Mediator)
                .AddTo(_customGameSeedPanel);

            new Button("Cancel")
                .SendOnClick<CancelCustomGameSeedRequest>(Mediator)
                .AddTo(_customGameSeedPanel);
        }

        public void Handle(CustomGameSeedEnterKeyRequest request)
        {
            if (_customSeed.Value.Length > 0)
                _startGameButton.OnClick.Invoke(_startGameButton);
        }

        public override void Show()
        {
            base.Show();
            _customSeed.IsFocused = true;
        }
    }
}