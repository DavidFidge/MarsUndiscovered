using System;
using System.Text;

using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class ConsoleView : BaseView<ConsoleViewModel, ConsoleData>
    {
        private TextInput _consoleEntry;
        private Paragraph _consoleHistory;

        public ConsoleView(
            ConsoleViewModel consoleViewModel
        ) : base(consoleViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            var containerPanel = new Panel(new Vector2(-1, 0.3f), PanelSkin.None, Anchor.BottomCenter, new Vector2(0f, -15f))
            {
                Padding = new Vector2(0f, 0f),
            };

            RootPanel.AddChild(containerPanel);

            var consolePanel = new Panel(new Vector2(0, 0))
            {
                Padding = new Vector2(30f, 30f)
            };

            containerPanel.AddChild(consolePanel);

            var consolePrompt = new Label(">", Anchor.AutoInlineNoBreak, new Vector2(0.03f, 0.2f))
            {
                TextStyle = FontStyle.Bold,
                Scale = 1.2f
            };

            consolePanel.AddChild(consolePrompt);

            _consoleEntry = new TextInput(false, new Vector2(0.96f, 0.2f), Anchor.AutoInlineNoBreak, null, PanelSkin.Simple)
            {
                Padding = new Vector2(0.1f, 0f),
                FillColor = Color.Black,
                Opacity = 128
            };

            _consoleEntry.OnValueChange = OnValueChange;

            consolePanel.AddChild(_consoleEntry);

            var hr = new HorizontalLine(Anchor.AutoInline);

            consolePanel.AddChild(hr);

            _consoleHistory = new Paragraph(String.Empty, Anchor.AutoInline, new Vector2(-1, 1f));

            consolePanel.AddChild(_consoleHistory);
        }

        private void OnValueChange(Entity entity)
        {
            _consoleEntry.Value = _consoleEntry.Value.Replace("`", String.Empty);
            Data.Command = _consoleEntry.Value;
        }

        public void FocusConsoleEntry()
        {
            _consoleEntry.IsFocused = true;
        }

        public override void Show()
        {
            base.Show();
            FocusConsoleEntry();
        }

        protected override void UpdateView()
        {
            _consoleEntry.Value = Data.Command;

            var stringBuilder = new StringBuilder();

            foreach (var lastCommand in Data.LastCommands)
            {
                if (!string.IsNullOrEmpty(lastCommand.Result))
                    stringBuilder.AppendLine(lastCommand.Result);

                stringBuilder.AppendLine(lastCommand.ToString());
            }

            _consoleHistory.Text = stringBuilder.ToString();

            base.UpdateView();
        }
    }
}