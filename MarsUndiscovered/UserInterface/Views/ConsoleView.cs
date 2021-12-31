using System;
using System.Text;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI;
using GeonBit.UI.DataTypes;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
                .WithPadding(Vector2.Zero)
                .WithAnchor(Anchor.BottomCenter)
                .WidthOfScreen()
                .Height(0.5f)
                .Opacity70Percent();

            RootPanel.AddChild(consolePanel);

            var consolePrompt = new Label(">")
                .Bold()
                .WithScale(1.2f)
                .WithSize(new Vector2(0.014f, 0.2f))
                .WithAnchor(Anchor.AutoInlineNoBreak)
                .NoPadding();

            consolePanel.AddChild(consolePrompt);

            _consoleEntry = new TextInput()
                .WithSize(new Vector2(0f, Constants.TextInputMinimalHeight * consolePanel.Size.Y))
                .WithAnchor(Anchor.AutoInlineNoBreak)
                .NoPadding()
                .WithSkin(PanelSkin.Simple)
                .Opacity70Percent();

            _consoleEntry.AddDisabledSpecialChar(SpecialChars.ArrowUp);
            _consoleEntry.AddDisabledSpecialChar(SpecialChars.ArrowDown);

            _consoleEntry.OnValueChange = OnValueChange;

            consolePanel.AddChild(_consoleEntry);

            var hr = new HorizontalLine(Anchor.AutoInline)
                .NoPadding();

            consolePanel.AddChild(hr);

            _consoleHistory = new SelectList()
                .NoSkin()
                .WithAnchor(Anchor.Auto)
                .NoPadding()
                .WithSize(new Vector2(0.99f, 0.88f));

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