using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using GoRogue.Pathing;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.UserInterface.Input;

using MediatR;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using SadRogue.Primitives;
using Button = GeonBit.UI.Entities.Button;
using Panel = GeonBit.UI.Entities.Panel;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.Views
{
    public class GameView : BaseGameView<GameViewModel, GameData>,
        IRequestHandler<OpenInGameOptionsRequest>,
        IRequestHandler<CloseInGameOptionsRequest>,
        IRequestHandler<OpenConsoleRequest>,
        IRequestHandler<CloseConsoleRequest>,
        IRequestHandler<OpenGameInventoryRequest>,
        IRequestHandler<CloseGameInventoryRequest>,
        IRequestHandler<LeftClickViewRequest>,
        IRequestHandler<RightClickViewRequest>,
        IRequestHandler<MoveUpRequest>,
        IRequestHandler<MoveUpLeftRequest>,
        IRequestHandler<MoveUpRightRequest>,
        IRequestHandler<MoveDownRequest>,
        IRequestHandler<MoveDownLeftRequest>,
        IRequestHandler<MoveDownRightRequest>,
        IRequestHandler<MoveLeftRequest>,
        IRequestHandler<MoveRightRequest>,
        IRequestHandler<MoveWaitRequest>,
        IRequestHandler<AutoExploreRequest>,
        IRequestHandler<EndRadioCommsRequest>,
        IRequestHandler<WizardModeNextLevelRequest>,
        IRequestHandler<WizardModePreviousLevelRequest>,
        IRequestHandler<HotBarItemRequest>,
        IRequestHandler<RefreshHotBarRequest>,
        IRequestHandler<LeftClickSquareChoiceGameViewRequest>,
        IRequestHandler<CloseSquareChoiceRequest>,
        IRequestHandler<SquareChoiceSelectSquareRequest>,
        IRequestHandler<SquareChoiceMouseHoverViewRequest>,
        IRequestHandler<RangeAttackEquippedWeaponRequest>,
        IRequestHandler<MoveSquareChoiceSelectionDownRequest>,
        IRequestHandler<MoveSquareChoiceSelectionDownLeftRequest>,
        IRequestHandler<MoveSquareChoiceSelectionDownRightRequest>,
        IRequestHandler<MoveSquareChoiceSelectionLeftRequest>,
        IRequestHandler<MoveSquareChoiceSelectionRightRequest>,
        IRequestHandler<MoveSquareChoiceSelectionUpRequest>,
        IRequestHandler<MoveSquareChoiceSelectionUpLeftRequest>,
        IRequestHandler<MoveSquareChoiceSelectionUpRightRequest>
    {
        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly InventoryGameView _inventoryGameView;
        private readonly GameViewGameOverKeyboardHandler _gameOverKeyboardHandler;
        private readonly GameViewGameOverMouseHandler _gameOverMouseHandler;
        private readonly GameViewRadioCommsKeyboardHandler _radioCommsKeyboardHandler;
        private readonly GameViewRadioCommsMouseHandler _gameViewRadioCommsMouseHandler;
        private readonly SquareChoiceGameViewKeyboardHandler _squareChoiceGameViewKeyboardHandler;
        private readonly SquareChoiceGameViewMouseHandler _squareChoiceGameViewMouseHandler;
        private readonly IStopwatchProvider _stopwatchProvider;
        private readonly Options _options;
        private Path _currentMovePath;
        private bool _isAutoExploring;
        private double _lastMoveTime = 0;
        private double _delayBetweenMove = 50;
        private Queue<RadioCommsItem> _radioCommsItems = new();
        private bool _isWaitingForRadioComms;

        protected Panel RadioCommsPanel;
        protected Panel HotBarPanel;
        protected RichParagraph RadioCommsMessage;
        protected RichParagraph RadioCommsSource;
        protected Image RadioCommsImage;
        protected AnimatedSprite RadioCommsAnimatedSprite;
        protected HotBarItemPanel[] HotBarPanelItems { get; set; }

        private InventoryItem _selectedItem;

        public GameView(
            GameViewModel gameViewModel,
            InGameOptionsView inGameOptionsView,
            ConsoleView consoleView,
            InventoryGameView inventoryGameView,
            GameViewGameOverKeyboardHandler gameOverKeyboardHandler,
            GameViewGameOverMouseHandler gameOverMouseHandler,
            GameViewRadioCommsKeyboardHandler radioCommsKeyboardHandler,
            GameViewRadioCommsMouseHandler gameViewRadioCommsMouseHandler,
            SquareChoiceGameViewKeyboardHandler squareChoiceGameViewKeyboardHandler,
            SquareChoiceGameViewMouseHandler squareChoiceGameViewMouseHandler,
            IGameCamera gameCamera,
            IStopwatchProvider stopwatchProvider,
            Options options
        )
            : base(gameCamera, gameViewModel)
        {
            _inGameOptionsView = inGameOptionsView;
            _consoleView = consoleView;
            _inventoryGameView = inventoryGameView;
            _gameOverKeyboardHandler = gameOverKeyboardHandler;
            _gameOverMouseHandler = gameOverMouseHandler;
            _radioCommsKeyboardHandler = radioCommsKeyboardHandler;
            _gameViewRadioCommsMouseHandler = gameViewRadioCommsMouseHandler;
            _squareChoiceGameViewKeyboardHandler = squareChoiceGameViewKeyboardHandler;
            _squareChoiceGameViewMouseHandler = squareChoiceGameViewMouseHandler;
            _stopwatchProvider = stopwatchProvider;
            _options = options;
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            
            CreateLayoutPanels();
            SetupInGameOptionsButton(LeftPanel);
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
            CreateRadioCommsPanel();
            CreateHotBarPanel();
            SetupConsole();
            SetupInventoryGame();
            SetupChildPanel(_inGameOptionsView);

            _stopwatchProvider.Start();
        }

        protected void CreateHotBarPanel()
        {
            HotBarPanel = new Panel()
                .Anchor(Anchor.BottomCenter)
                .SkinNone()
                .NoPadding()
                .Height(UiConstants.HotBarHeight)
                .WidthOfContainer();
            
            GameViewPanel.AddChild(HotBarPanel);

            HotBarPanelItems = new HotBarItemPanel[10];
            
            for (var i = 0; i < 10; i++)
            {
                var key = i + 1;
                
                if (key == 10)
                    key = 0;
                
                var hotBarItemPanel = new HotBarItemPanel(Assets, GetHotBarKey(key))
                    .Anchor(Anchor.AutoInlineNoBreak)
                    .SkinAlternative()
                    .NoPadding()
                    .Width(0.05f)
                    .HeightOfParent();
                
                var separator = new Panel().Anchor(Anchor.AutoInlineNoBreak)
                    .SkinNone()
                    .NoPadding()
                    .Width(0.01f)
                    .HeightOfParent();
                
                HotBarPanelItems[i] = hotBarItemPanel;
                HotBarPanel.AddChild(hotBarItemPanel);
                
                if (i != 9)
                    HotBarPanel.AddChild(separator);
            }
        }

        private Keys GetHotBarKey(int i)
        {
            return i switch
            {
                1 => Keys.D1,
                2 => Keys.D2,
                3 => Keys.D3,
                4 => Keys.D4,
                5 => Keys.D5,
                6 => Keys.D6,
                7 => Keys.D7,
                8 => Keys.D8,
                9 => Keys.D9,
                0 => Keys.D0,
                _ => throw new Exception()
            };
        }

        protected void CreateRadioCommsPanel()
        {
            RadioCommsPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Skin(PanelSkin.Alternative)
                .Height(UiConstants.RadioCommsPanelHeight)
                .WidthOfContainer();

            BottomPanel.AddChild(RadioCommsPanel);

            RadioCommsSource = new RichParagraph()
                .Anchor(Anchor.AutoCenter)
                .NoPadding();
            
            RadioCommsPanel.AddChild(RadioCommsSource);

            RadioCommsImage = new Image()
                .Anchor(Anchor.AutoInline)
                .Width(256)
                .Height(256)
                .NoPadding();
            
            RadioCommsPanel.AddChild(RadioCommsImage);

            var spacer = new Panel()
                .Anchor(Anchor.AutoInline)
                .NoPadding()
                .SkinNone()
                .Width(0.01f);
            
            RadioCommsPanel.AddChild(spacer);
            
            RadioCommsMessage = new RichParagraph()
                .Anchor(Anchor.AutoInlineNoBreak)
                .Width(0.87f)
                .NoPadding();
            
            RadioCommsPanel.AddChild(RadioCommsMessage);

            RadioCommsPanel.Hidden();
        }

        public void NewGame(ulong? seed = null)
        {
            ResetViews();
            _viewModel.NewGame(seed);
        }

        public void LoadGame(string filename)
        {
            ResetViews();
            _viewModel.LoadGame(filename);
        }

        private void SetupInGameOptionsButton(Panel leftPanel)
        {
            var menuButton = new Button(
                    "-",
                    ButtonSkin.Default,
                    Anchor.Auto,
                    new Vector2(50, 50)
                )
                .SendOnClick<OpenInGameOptionsRequest>(Mediator)
                .NoPadding();

            leftPanel.AddChild(menuButton);
        }

        private void SetupConsole()
        {
            _consoleView.Initialize();

            RootPanel.AddChild(_consoleView.RootPanel);
        }

        private void SetupInventoryGame()
        {
            _inventoryGameView.Initialize();

            RootPanel.AddChild(_inventoryGameView.RootPanel);
        }

        private void StopAutoMovement()
        {
            _currentMovePath = null;
            _isAutoExploring = false;
        }

        public Task<Unit> Handle(OpenInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _inGameOptionsView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _inGameOptionsView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(OpenConsoleRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _consoleView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseConsoleRequest request, CancellationToken cancellationToken)
        {
            _consoleView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(LeftClickViewRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            _currentMovePath = _viewModel.GetPathToDestination(ray);

            return Unit.Task;
        }

        public Task<Unit> Handle(RightClickViewRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveLeftRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveRightRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpLeftRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRightRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownLeftRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRightRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveWaitRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();

            RefreshHotBars();

            if (_viewModel.PlayerStatus.IsDead || _viewModel.PlayerStatus.IsVictorious)
            {
                _viewModel.WriteAndSendMorgue();
                _viewModel.MapViewModel.ClearHover();
                _viewModel.MapViewModel.UpdateAllTiles();

                if (_viewModel.PlayerStatus.IsDead)
                {
                    GameInputService?.ChangeInput(_gameOverMouseHandler, _gameOverKeyboardHandler);
                    StatusParagraph.Text = DelimitWithDashes("YOU ARE DEAD. PRESS SPACE OR CLICK TO EXIT GAME.");
                }
                else if (_viewModel.PlayerStatus.IsVictorious)
                {
                    GameInputService?.ChangeInput(_gameOverMouseHandler, _gameOverKeyboardHandler);
                    StatusParagraph.Text = DelimitWithDashes("YOU ARE VICTORIOUS! PRESS SPACE OR CLICK TO EXIT GAME.");
                }
            }
            else
            {
                ProcessRadioComms();
            }
        }

        protected void ProcessRadioComms()
        {
            foreach (var item in _viewModel.GetNewRadioCommsItems()) 
                _radioCommsItems.Enqueue(item);

            if (_options.SkipRadioComms)
                _radioCommsItems.Clear();
            
            if (_radioCommsItems.Any())
            {
                _isAutoExploring = false;
                _isWaitingForRadioComms = true;
                GameInputService?.ChangeInput(_gameViewRadioCommsMouseHandler, _radioCommsKeyboardHandler);
                ProcessNextRadioComm();
            }
        }

        private void ProcessNextRadioComm()
        {
            if (_radioCommsItems.Any())
            {
                var nextRadioComms = _radioCommsItems.Dequeue();

                StatusParagraph.Text = DelimitWithDashes("PRESS SPACE OR CLICK TO CONTINUE");

                RadioCommsMessage.Text = nextRadioComms.Message;
                RadioCommsSource.Text = nextRadioComms.Source;

                var radioCommsSpriteSheet = Assets.GetRadioCommsSpriteSheet(nextRadioComms.RadioCommsType);

                RadioCommsAnimatedSprite = new AnimatedSprite(radioCommsSpriteSheet);
                RadioCommsAnimatedSprite.Play("talk");
                RadioCommsImage.Texture = RadioCommsAnimatedSprite.TextureRegion.Texture;
                RadioCommsImage.SourceRectangle = RadioCommsAnimatedSprite.TextureRegion.Bounds;
                RadioCommsPanel.Visible();
            }
            else
            {
                StatusParagraph.Text = String.Empty;
                GameInputService?.RevertInputUpToAndIncluding(_gameViewRadioCommsMouseHandler, _radioCommsKeyboardHandler);
                RadioCommsPanel.Hidden();
                _isWaitingForRadioComms = false;
            }
        }

        public override Task Handle(MouseHoverViewNotification notification, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            base.Handle(notification, cancellationToken);

            var ray = _gameCamera.GetPointerRay(notification.X, notification.Y);

            _viewModel.MapViewModel.ShowHover(ray);

            return Unit.Task;
        }

        public override void Update()
        {
            base.Update();

            UpdateRadioCommsAnimation();

            if (_viewModel.IsAnimating)
                return;

            if (_currentMovePath != null)
            {
                if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastMoveTime > _delayBetweenMove)
                {
                    _lastMoveTime = _stopwatchProvider.Elapsed.TotalMilliseconds;
                    var isComplete = _viewModel.Move(_currentMovePath);

                    if (isComplete)
                        _currentMovePath = null;
                }
            }
            else if (_isAutoExploring)
            {
                if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastMoveTime > _delayBetweenMove)
                {
                    _lastMoveTime = _stopwatchProvider.Elapsed.TotalMilliseconds;
                    var autoExploreResult = _viewModel.AutoExplore();

                    if (autoExploreResult.MovementInterrupted || autoExploreResult.Path.Length == 0)
                        _isAutoExploring = false;
                }
            }
        }

        private void UpdateRadioCommsAnimation()
        {
            if (_isWaitingForRadioComms)
            {
                RadioCommsAnimatedSprite.Update(_viewModel.GameTimeService.GameTime);
                RadioCommsImage.SourceRectangle = RadioCommsAnimatedSprite.TextureRegion.Bounds;
            }
        }

        public Task<Unit> Handle(OpenGameInventoryRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            StopAutoMovement();

            var rect = BottomPanel.CalcDestRect();
            HotBarPanel.Offset(0, -rect.Height);
                
            _inventoryGameView.SetInventoryMode(request.InventoryMode);
            _inventoryGameView.Show();
            
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseGameInventoryRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;

            HotBarPanel.Offset(0, 0);
            _inventoryGameView.Hide();
            
            return Unit.Task;
        }

        public Task<Unit> Handle(AutoExploreRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            _isAutoExploring = true;
            return Unit.Task;
        }

        protected override void ResetViews()
        {
            base.ResetViews();
            _isAutoExploring = false;
            _currentMovePath = null;
        }

        public Task<Unit> Handle(EndRadioCommsRequest request, CancellationToken cancellationToken)
        {
            ProcessNextRadioComm();
            return Unit.Task;
        }

        public Task<Unit> Handle(WizardModeNextLevelRequest request, CancellationToken cancellationToken)
        {
            _viewModel.ForceNextLevel();
            return Unit.Task;
        }

        public Task<Unit> Handle(WizardModePreviousLevelRequest request, CancellationToken cancellationToken)
        {
            _viewModel.ForcePreviousLevel();
            return Unit.Task;
        }

        public Task<Unit> Handle(HotBarItemRequest request, CancellationToken cancellationToken)
        {
            var hotBarPanel = HotBarPanelItems.First(i => i.Key == request.Key);

            if (hotBarPanel.InventoryItem == null)
                return Unit.Task;
            
            if (hotBarPanel.InventoryItem.CanRangeAttack)
            {
                EnterRangeAttackMode(hotBarPanel.InventoryItem);
            }
            else if (hotBarPanel.InventoryItem.CanApply)
            {
                _viewModel.ApplyRequest(request.Key);
            }
            
            return Unit.Task;
        }

        private void EnterRangeAttackMode(InventoryItem inventoryItem)
        {
            _selectedItem = inventoryItem;
                
            StatusParagraph.Text = "Fire at what?";
            GameInputService.ChangeInput(_squareChoiceGameViewMouseHandler, _squareChoiceGameViewKeyboardHandler);
        }

        public Task<Unit> Handle(RefreshHotBarRequest request, CancellationToken cancellationToken)
        {
            if (request.ItemId != null)
            {
                var hotBarItems = _viewModel.GetHotBarItems();

                // Single refresh, supports adding a new item or removing an existing item
                var hotBarPanel = HotBarPanelItems.FirstOrDefault(i => i.InventoryItem?.ItemId == request.ItemId);
                hotBarPanel?.SetNoInventory();

                var hotBarPanelItem = hotBarItems.FirstOrDefault(i => i.ItemId == request.ItemId);

                if (hotBarPanelItem != null && hotBarPanelItem.HotBarKey != Keys.None)
                {
                    hotBarPanel = HotBarPanelItems.First(i => i.Key == hotBarPanelItem.HotBarKey);
                    hotBarPanel.SetInventoryItem(hotBarPanelItem);
                }
            }
            else
            {
                // Full refresh
                RefreshHotBars();
            }
            
            return Unit.Task;
        }

        private void RefreshHotBars()
        {
            // Full refresh
            var hotBarItems = _viewModel.GetHotBarItems();

            foreach (var hotBarPanel in HotBarPanelItems)
            {
                var hotBarPanelItem = hotBarItems.FirstOrDefault(i => i.HotBarKey == hotBarPanel.Key);

                if (hotBarPanelItem != null)
                    hotBarPanel.SetInventoryItem(hotBarPanelItem);
                else
                    hotBarPanel.SetNoInventory();
            }
        }

        public Task<Unit> Handle(LeftClickSquareChoiceGameViewRequest request, CancellationToken cancellationToken)
        {
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            var point = _viewModel.MapViewModel.MousePointerRayToMapPosition(ray);

            DoRangedAttack(cancellationToken, point);
            
            return Unit.Task;
        }

        private void DoRangedAttack(CancellationToken cancellationToken, Point? point)
        {
            if (point != null)
            {
                _viewModel.DoRangedAttack(_selectedItem, point.Value);

                _viewModel.MapViewModel.ClearHover();

                Mediator.Send(new CloseSquareChoiceRequest(), cancellationToken);
            }
        }

        public Task<Unit> Handle(CloseSquareChoiceRequest request, CancellationToken cancellationToken)
        {
            StatusParagraph.Text = String.Empty;
            GameInputService.RevertInputUpToAndIncluding(_squareChoiceGameViewMouseHandler,
                _squareChoiceGameViewKeyboardHandler);

            _selectedItem = null;

            return Unit.Task;
        }

        public Task<Unit> Handle(SquareChoiceMouseHoverViewRequest request, CancellationToken cancellationToken)
        {  
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);

            _viewModel.MapViewModel.ShowHoverForSquareChoice(ray);

            return Unit.Task;
        }

        public Task<Unit> Handle(RangeAttackEquippedWeaponRequest request, CancellationToken cancellationToken)
        {
            var equippedWeapon = _viewModel.GetEquippedWeapon();

            if (equippedWeapon == null)
            {
                _viewModel.MessageStatus.AddMessages("No weapon equipped");
                
                return Unit.Task;
            }

            if (equippedWeapon.CanRangeAttack == false)
            {
                _viewModel.MessageStatus.AddMessages("Equipped weapon cannot perform a ranged attack");

                return Unit.Task;
            }
            
            EnterRangeAttackMode(equippedWeapon);
            
            return Unit.Task;
        }

        private void MoveSquareChoice(Direction requestDirection)
        {
            _viewModel.MoveSquareChoice(requestDirection); 
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionDownRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionDownLeftRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionDownRightRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionLeftRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionRightRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionUpRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionUpLeftRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveSquareChoiceSelectionUpRightRequest request, CancellationToken cancellationToken)
        {
            MoveSquareChoice(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(SquareChoiceSelectSquareRequest request, CancellationToken cancellationToken)
        {
            var point = _viewModel.MapViewModel.MouseHoverPath?.End;
            DoRangedAttack(cancellationToken, point);

            return Unit.Task;
        }
    }
}