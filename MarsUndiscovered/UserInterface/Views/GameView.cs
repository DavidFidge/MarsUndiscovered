using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.Input;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using SadRogue.Primitives;
using Button = GeonBit.UI.Entities.Button;
using Color = Microsoft.Xna.Framework.Color;
using Panel = GeonBit.UI.Entities.Panel;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.Views
{
    public class GameView : BaseGameCoreView<GameViewModel, GameData>,
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
        IRequestHandler<SquareChoiceNextTargetRequest>,
        IRequestHandler<SquareChoicePreviousTargetRequest>,
        IRequestHandler<SquareChoiceMouseHoverViewRequest>,
        IRequestHandler<RangeAttackEquippedWeaponRequest>,
        IRequestHandler<MoveSquareChoiceSelectionDownRequest>,
        IRequestHandler<MoveSquareChoiceSelectionDownLeftRequest>,
        IRequestHandler<MoveSquareChoiceSelectionDownRightRequest>,
        IRequestHandler<MoveSquareChoiceSelectionLeftRequest>,
        IRequestHandler<MoveSquareChoiceSelectionRightRequest>,
        IRequestHandler<MoveSquareChoiceSelectionUpRequest>,
        IRequestHandler<MoveSquareChoiceSelectionUpLeftRequest>,
        IRequestHandler<MoveSquareChoiceSelectionUpRightRequest>,
        INotificationHandler<MouseHoverViewNotification>
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
        private double _lastMoveTime;
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
        
        private SelectList _messageLog;
        protected Panel LeftPanel;
        protected Panel BottomPanel;
        protected PlayerPanel PlayerPanel;
        protected IList<MonsterPanel> MonsterPanels = new List<MonsterPanel>();
        protected RichParagraph StatusParagraph;
        protected RichParagraph HoverPanelLeftTooltip;
        protected RichParagraph HoverPanelRightTooltip;
        protected RichParagraph AmbientParagraph;
        protected Panel GameViewPanel { get; set; }
        protected Panel HoverPanelLeft { get; set; }
        protected Panel HoverPanelRight { get; set; }

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
        
        
        protected void CreateLayoutPanels()
        {
            // This creates a three-sectioned layout
            // A left panel for game information
            // A bottom panel for messages
            // The rest of the space is for the game view
            LeftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(UiConstants.LeftPanelWidth)
                .SkinSimple()
                .Height(UiConstants.HeightOfParent)
                .NoPadding();
            
            RootPanel.AddChild(LeftPanel);

            BottomPanel = new Panel()
                .Anchor(Anchor.BottomRight)
                .Width(UiConstants.GameViewPanelWidth)
                .SkinSimple()
                .NoPadding()
                .Offset(new Vector2(UiConstants.LeftPanelWidth, 0f))
                .Height(UiConstants.BottomPanelHeight);

            RootPanel.AddChild(BottomPanel);

            GameViewPanel = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(UiConstants.GameViewPanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(UiConstants.GameViewPanelHeight);

            // This creates two sections in the game view that is used for 'popups' like game inventory
            HoverPanelLeft = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(0.45f)
                .Skin(PanelSkin.Alternative)
                .AutoHeight()
                .Hidden();

            GameViewPanel.AddChild(HoverPanelLeft);

            HoverPanelLeftTooltip = new RichParagraph();

            HoverPanelLeft.AddChild(HoverPanelLeftTooltip);

            HoverPanelRight = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(0.45f)
                .Skin(PanelSkin.Alternative)
                .AutoHeight()
                .Hidden();

            HoverPanelRightTooltip = new RichParagraph();

            HoverPanelRight.AddChild(HoverPanelRightTooltip);

            GameViewPanel.AddChild(HoverPanelRight);

            RootPanel.AddChild(GameViewPanel);
        }

        protected void CreateStatusPanel()
        {
            StatusParagraph = new RichParagraph()
                .Anchor(Anchor.BottomCenter)
                .NoPadding()
                .Height(0.1f);

            StatusParagraph.BackgroundColor = Color.Black;

            BottomPanel.AddChild(StatusParagraph);
        }
        
        protected void CreateAmbientPanel()
        {
            AmbientParagraph = new RichParagraph()
                .Anchor(Anchor.BottomLeft)
                .NoPadding()
                .Height(0.5f);

            AmbientParagraph.BackgroundColor = Color.Black;

            LeftPanel.AddChild(AmbientParagraph);

            AmbientParagraph.Text = "Welcome to Mars Undiscovered!";
        } 
     
        protected void CreateMessageLog()
        {
            _messageLog = new SelectList()
                .SkinSimple()
                .Anchor(Anchor.TopLeft)
                .Height(UiConstants.MessageLogHeight)
                .NoPadding();

            _messageLog.ExtraSpaceBetweenLines = -14;
            _messageLog.LockSelection = true;
            BottomPanel.AddChild(_messageLog);

            _messageLog.OnListChange = entity =>
            {
                var list = (SelectList)entity;
                if (list.Count > 100)
                    list.RemoveItem(0);
            };
        }

        protected void CreatePlayerPanel()
        {
            PlayerPanel = new PlayerPanel(Assets);
            PlayerPanel.AddAsChildTo(LeftPanel);
        }

        protected void ResetViews()
        {
            _gameCamera.Reset();
            _messageLog.ClearItems();
            PlayerPanel.Reset();
            
            _isAutoExploring = false;
            _currentMovePath = null;
        }
        
        private void UpdateMessageLog()
        {
            var newMessages = _viewModel.MessageStatus.GetUnprocessedMessages();

            if (newMessages.Any())
            {
                foreach (var message in newMessages)
                    _messageLog.AddItem(message);

                _messageLog.scrollToEnd();
            }
        }

        private void UpdatePlayerStatus()
        {
            PlayerPanel.Update(_viewModel.PlayerStatus);
            AmbientParagraph.Text = _viewModel.PlayerStatus.AmbientText;
        }

        protected void UpdateMonsterStatus()
        {
            foreach (var panel in MonsterPanels)
            {
                panel.RemoveFromParent();
            }

            var newMonsterStatuses = _viewModel.MonsterStatusInView
                .OrderBy(m => m.DistanceFromPlayer)
                .ToList();

            var mergedMonsterPanelStatusQuery = (
                from monsterStatus in newMonsterStatuses
                join panel in MonsterPanels on monsterStatus.ID equals panel.ActorStatus.ID into gj
                from subPanel in gj.DefaultIfEmpty()
                select new
                {
                    MonsterStatus = monsterStatus,
                    MonsterPanel = subPanel ?? new MonsterPanel(monsterStatus, Assets)
                }).ToList();

            MonsterPanels.Clear();

            foreach (var monsterJoin in mergedMonsterPanelStatusQuery)
            {
                monsterJoin.MonsterPanel.Update(monsterJoin.MonsterStatus);

                MonsterPanels.Add(monsterJoin.MonsterPanel);
                monsterJoin.MonsterPanel.AddAsChildTo(LeftPanel);
            }
        }

        protected string DelimitWithDashes(string text)
        {
            return $"--- {text} ---";
        }
        
        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            
            CreateLayoutPanels();
            SetupInGameOptionsButton(LeftPanel);
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
            CreateAmbientPanel();
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

        public void Handle(OpenInGameOptionsRequest request)
        {
            StopAutoMovement();
            _inGameOptionsView.Show();
        }

        public void Handle(CloseInGameOptionsRequest request)
        {
            StopAutoMovement();
            _inGameOptionsView.Hide();
        }

        public void Handle(OpenConsoleRequest request)
        {
            StopAutoMovement();
            _consoleView.Show();
        }

        public void Handle(CloseConsoleRequest request)
        {
            _consoleView.Hide();
        }

        public void Handle(LeftClickViewRequest request)
        {
            StopAutoMovement();
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            _currentMovePath = _viewModel.GetPathToDestination(ray);
        }

        public void Handle(RightClickViewRequest request)
        {
            StopAutoMovement();
        }

        public void Handle(MoveUpRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveDownRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveLeftRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveRightRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveUpLeftRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveUpRightRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveDownLeftRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveDownRightRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        public void Handle(MoveWaitRequest request)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();
            
            StatusParagraph.Text = String.Empty;

            UpdateMonsterStatus();
            UpdateMessageLog();
            UpdatePlayerStatus();
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

        public void Handle(MouseHoverViewNotification notification)
        {
            if (!IsVisible)
                return;
            
            if (!IsVisible)
                return;

            var ray = _gameCamera.GetPointerRay(notification.X, notification.Y);

            var gameObjectInformation = _viewModel.GetGameObjectTooltipAt(ray);

            if (!String.IsNullOrEmpty(gameObjectInformation))
            {
                var direction = _viewModel.GetMapQuadrantOfRay(ray);

                if (direction == Direction.Right || direction == Direction.DownRight || direction == Direction.UpRight)
                {
                    HoverPanelLeftTooltip.Text = gameObjectInformation;
                    HoverPanelLeft.Visible = true;
                    HoverPanelRight.Visible = false;
                }
                else
                {
                    HoverPanelRightTooltip.Text = gameObjectInformation;
                    HoverPanelLeft.Visible = false;
                    HoverPanelRight.Visible = true;
                }
            }
            else
            {
                HoverPanelLeft.Visible = false;
                HoverPanelRight.Visible = false;
            }

            _viewModel.MapViewModel.ShowHover(ray);
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

        public void Handle(OpenGameInventoryRequest request)
        {
            if (!IsVisible)
                return;
            
            StopAutoMovement();

            var rect = BottomPanel.CalcDestRect();
            HotBarPanel.Offset(0, -rect.Height);
                
            _inventoryGameView.SetInventoryMode(request.InventoryMode);
            _inventoryGameView.Show();
        }

        public void Handle(CloseGameInventoryRequest request)
        {
            if (!IsVisible)
                return;

            HotBarPanel.Offset(0, 0);
            _inventoryGameView.Hide();
        }

        public void Handle(AutoExploreRequest request)
        {
            if (!IsVisible)
                return;
            
            _isAutoExploring = true;
        }
        
        public void Handle(EndRadioCommsRequest request)
        {
            ProcessNextRadioComm();
        }

        public void Handle(WizardModeNextLevelRequest request)
        {
            _viewModel.ForceNextLevel();
        }

        public void Handle(WizardModePreviousLevelRequest request)
        {
            _viewModel.ForcePreviousLevel();
        }

        public void Handle(HotBarItemRequest request)
        {
            var hotBarPanel = HotBarPanelItems.First(i => i.Key == request.Key);

            if (hotBarPanel.InventoryItem == null)
                return;
            
            if (hotBarPanel.InventoryItem.CanRangeAttack)
            {
                EnterRangeAttackMode(hotBarPanel.InventoryItem);
            }
            else if (hotBarPanel.InventoryItem.CanApply)
            {
                _viewModel.ApplyRequest(request.Key);
            }
        }

        private void EnterRangeAttackMode(InventoryItem inventoryItem)
        {
            _selectedItem = inventoryItem;
                
            StatusParagraph.Text = "Fire at what?";
            GameInputService.ChangeInput(_squareChoiceGameViewMouseHandler, _squareChoiceGameViewKeyboardHandler);

            Mediator.Send(new SquareChoiceNextTargetRequest());
        }

        public void Handle(RefreshHotBarRequest request)
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

        public void Handle(LeftClickSquareChoiceGameViewRequest request)
        {
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            var point = _viewModel.MapViewModel.MousePointerRayToMapPosition(ray);

            DoRangedAttack(point);
        }

        private void DoRangedAttack(Point? point)
        {
            if (point != null)
            {
                _viewModel.DoRangedAttack(_selectedItem, point.Value);

                _viewModel.MapViewModel.ClearHover();

                Mediator.Send(new CloseSquareChoiceRequest());
            }
        }

        public void Handle(CloseSquareChoiceRequest request)
        {
            StatusParagraph.Text = String.Empty;
            GameInputService.RevertInputUpToAndIncluding(_squareChoiceGameViewMouseHandler,
                _squareChoiceGameViewKeyboardHandler);

            _selectedItem = null;
        }

        public void Handle(SquareChoiceMouseHoverViewRequest request)
        {  
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);

            _viewModel.MapViewModel.ShowHoverForSquareChoice(ray);
        }

        public void Handle(RangeAttackEquippedWeaponRequest request)
        {
            var equippedWeapon = _viewModel.GetEquippedWeapon();

            if (equippedWeapon == null)
            {
                _viewModel.MessageStatus.AddMessages("No weapon equipped");
                
                return;
            }

            if (equippedWeapon.CanRangeAttack == false)
            {
                _viewModel.MessageStatus.AddMessages("Equipped weapon cannot perform a ranged attack");

                return;
            }
            
            EnterRangeAttackMode(equippedWeapon);
        }

        private void MoveSquareChoice(Direction requestDirection)
        {
            _viewModel.MoveSquareChoice(requestDirection); 
        }

        public void Handle(MoveSquareChoiceSelectionDownRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionDownLeftRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionDownRightRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionLeftRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionRightRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionUpRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionUpLeftRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(MoveSquareChoiceSelectionUpRightRequest request)
        {
            MoveSquareChoice(request.Direction);
        }

        public void Handle(SquareChoiceSelectSquareRequest request)
        {
            var point = _viewModel.MapViewModel.MouseHoverPath?.End;
            DoRangedAttack(point);
            _viewModel.RetainedSquareChoiceMonsterId = _viewModel.CurrentSquareChoiceMonsterId;
            _viewModel.CurrentSquareChoiceMonsterId = null;
        }

        public void Handle(SquareChoiceNextTargetRequest request)
        {
            GetNextSquareChoice(false);
        }
        
        public void Handle(SquareChoicePreviousTargetRequest request)
        {
            GetNextSquareChoice(true);
        }

        private void GetNextSquareChoice(bool reverse)
        {
            if (_viewModel.CurrentSquareChoiceMonsterId == null)
                _viewModel.CurrentSquareChoiceMonsterId = _viewModel.RetainedSquareChoiceMonsterId;
            
            if (!MonsterPanels.Any())
                return;
            
            var monsterPanels = MonsterPanels.ToList();
            
            if (reverse)
                monsterPanels.Reverse();

            var panels = monsterPanels;
            
            if (_viewModel.CurrentSquareChoiceMonsterId != null)
                panels = monsterPanels
                    .SkipWhile(m => m.ActorStatus.ID != _viewModel.CurrentSquareChoiceMonsterId
                        )
                    .ToList();

            if (!panels.Any())
                panels = monsterPanels;
            else
                panels = panels.Skip(1).ToList();

            var panel = panels.FirstOrDefault() ?? monsterPanels.FirstOrDefault();

            if (panel == null)
                return;

            _viewModel.CurrentSquareChoiceMonsterId = panel.ActorStatus.ID;
            _viewModel.MapViewModel.ShowHoverForSquareChoice(panel.ActorStatus.Position);
        }
    }
}