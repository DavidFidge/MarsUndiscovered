﻿using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI;
using GeonBit.UI.Entities;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class ConsoleView : BaseMarsUndiscoveredView<ConsoleViewModel, ConsoleData>
    {
        private TextInput _consoleEntry;
        private SelectList _consoleHistory;

        public ConsoleView(
            ConsoleViewModel consoleViewModel
        ) : base(consoleViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            var consolePanel = new Panel()
                .Padding(Vector2.Zero)
                .Anchor(Anchor.BottomCenter)
                .WidthOfContainer()
                .Height(0.5f)
                .OpacityPercent(70);

            RootPanel.AddChild(consolePanel);

            var consolePrompt = new Label(">")
                .Bold()
                .Scale(1.2f)
                .Size(new Vector2(0.014f, 0.2f))
                .Anchor(Anchor.AutoInlineNoBreak)
                .NoPadding();

            consolePanel.AddChild(consolePrompt);

            _consoleEntry = new TextInput()
                .Size(new Vector2(0f, UiConstants.TextInputMinimalHeight * consolePanel.Size.Y))
                .Anchor(Anchor.AutoInlineNoBreak)
                .NoPadding()
                .Skin(PanelSkin.Alternative)
                .OpacityPercent(70);

            _consoleEntry.AddDisabledSpecialChar(SpecialChars.ArrowUp);
            _consoleEntry.AddDisabledSpecialChar(SpecialChars.ArrowDown);

            _consoleEntry.OnValueChange = OnValueChange;

            consolePanel.AddChild(_consoleEntry);

            var hr = new HorizontalLine(Anchor.AutoInline)
                .NoPadding();

            consolePanel.AddChild(hr);

            _consoleHistory = new SelectList()
                .SkinNone()
                .Anchor(Anchor.Auto)
                .NoPadding()
                .Size(new Vector2(1f, 0.88f));

            _consoleHistory.ExtraSpaceBetweenLines = -30;

            consolePanel.AddChild(_consoleHistory);
        }

        private void OnValueChange(Entity entity)
        {
            _consoleEntry.Value = _consoleEntry.Value.Replace("`", String.Empty);
            Data.Command = _consoleEntry.Value;
        }

        public override void Show()
        {
            base.Show();
            _consoleEntry.IsFocused = true;
        }

        protected override void ViewModelChanged()
        {
            _consoleEntry.Value = Data.Command;
            _consoleEntry.Caret = -1;

            _consoleHistory.ClearItems();

            foreach (var lastCommand in Data.LastCommands)
            {
                if (!string.IsNullOrEmpty(lastCommand.Result))
                    _consoleHistory.AddItem(lastCommand.Result);

                _consoleHistory.AddItem(lastCommand.ToString());
            }

            base.ViewModelChanged();
        }
    }
}