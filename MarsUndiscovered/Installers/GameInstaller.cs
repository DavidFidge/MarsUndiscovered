using Augmented.Components;
using Augmented.Graphics;
using Augmented.Graphics.Camera;
using Augmented.Graphics.Models;
using Augmented.Graphics.TerrainSpace;
using Augmented.Interfaces;
using Augmented.Messages;
using Augmented.Messages.Console;
using Augmented.UserInterface.Data;
using Augmented.UserInterface.Input;
using Augmented.UserInterface.Input.CameraMovementSpace;
using Augmented.UserInterface.Screens;
using Augmented.UserInterface.ViewModels;
using Augmented.UserInterface.Views;

using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Graphics.Camera;
using DavidFidge.MonoGame.Core.Graphics.Cylinder;
using DavidFidge.MonoGame.Core.Graphics.Models;
using DavidFidge.MonoGame.Core.Graphics.Terrain;
using DavidFidge.MonoGame.Core.Graphics.Trees;
using DavidFidge.MonoGame.Core.Installers;
using DavidFidge.MonoGame.Core.Interfaces.Components;
using DavidFidge.MonoGame.Core.Interfaces.ConsoleCommands;
using DavidFidge.MonoGame.Core.Interfaces.Graphics;
using DavidFidge.MonoGame.Core.Interfaces.UserInterface;
using DavidFidge.MonoGame.Core.Messages;
using DavidFidge.MonoGame.Core.UserInterface;
using DavidFidge.Monogame.Core.View;
using DavidFidge.Monogame.Core.View.Installers;

using InputHandlers.Keyboard;
using InputHandlers.Mouse;

using MediatR;

namespace Augmented.Installers
{
    public class GameInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(new CoreInstaller());
            container.Install(new ViewInstaller());

            RegisterTitleView(container, store);
            RegisterOptionsView(container, store);
            RegisterVideoOptionsView(container, store);
            RegisterInGameOptionsView(container, store);
            RegisterConsoleView(container, store);
            RegisterGameView(container, store);
            RegisterGameSpeedView(container, store);

            RegisterKeyboardHandlers(container);
            RegisterMouseHandlers(container);

            container.Register(

                Component.For<IAssetProvider>()
                    .ImplementedBy<AssetProvider>(),

                Component.For<IGame>()
                    .Forward<IRequestHandler<ExitGameRequest, Unit>>()
                    .ImplementedBy<AugmentedGame>(),

                Component.For<IScreenManager>()
                    .Forward<IRequestHandler<NewGameRequest, Unit>>()
                    .Forward<IRequestHandler<ExitCurrentGameRequest, Unit>>()
                    .ImplementedBy<ScreenManager>(),
                
                Classes.FromThisAssembly()
                    .BasedOn<Screen>(),

                Component.For<GameView3D>()
                    .Forward<IRequestHandler<Move3DViewRequest, Unit>>()
                    .Forward<IRequestHandler<Zoom3DViewRequest, Unit>>()
                    .Forward<IRequestHandler<Rotate3DViewRequest, Unit>>()
                    .Forward<IRequestHandler<Select3DViewRequest, Unit>>()
                    .Forward<IRequestHandler<Action3DViewRequest, Unit>>()
                    .DependsOn(Dependency.OnComponent<IGameCamera, StrategyGameCamera>()
                ),

                Component.For<IHeightMapGenerator>()
                    .ImplementedBy<HeightMapGenerator>(),

                Component.For<Terrain>()
                    .LifeStyle.Transient,

                Component.For<AugmentedEntity>()
                    .LifeStyle.Transient,

                Component.For<IAugmentedEntityFactory>()
                    .AsFactory(),

                Component.For<IAugmentedModelDrawer>()
                    .ImplementedBy<AugmentedModelDrawer>()
                    .LifeStyle.Transient,

                Component.For<ISelectionModelDrawer>()
                    .ImplementedBy<SelectionModelDrawer>()
                    .LifeStyle.Transient,

                Component.For<Cylinder>()
                    .LifeStyle.Transient,

                Component.For<Tree>()
                    .LifeStyle.Transient,

                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .LifestyleTransient()
                    .WithServiceDefaultInterfaces(),

                Component.For<IAugmentedGameWorld>()
                    .ImplementedBy<AugmentedGameWorld>(),

                Component.For<IActionMapStore>()
                    .ImplementedBy<DefaultActionMapStore>()
            );
        }

        private void RegisterMouseHandlers(IWindsorContainer container)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IMouseHandler>()
                    .ConfigureFor<NullMouseHandler>(c => c.IsDefault())
                    .WithServiceDefaultInterfaces()
            );
        }

        private void RegisterKeyboardHandlers(IWindsorContainer container)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IKeyboardHandler>()
                    .ConfigureFor<NullKeyboardHandler>(c => c.IsDefault())
                    .ConfigureFor<GameViewKeyboardHandler>(c => c.DependsOn(Dependency.OnComponent<ICameraMovement, StrategyCameraMovement>()))
                    .WithServiceDefaultInterfaces()
            );
        }

        private void RegisterTitleView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<TitleView>()
                    .Forward<IRequestHandler<OptionsButtonClickedRequest, Unit>>()
                    .Forward<IRequestHandler<CloseOptionsViewRequest, Unit>>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, TitleViewKeyboardHandler>()),

                Component.For<TitleViewModel>()
            );
        }

        private void RegisterOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<OptionsView>()
                    .Forward<IRequestHandler<OpenVideoOptionsRequest, Unit>>()
                    .Forward<IRequestHandler<CloseVideoOptionsRequest, Unit>>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, OptionsKeyboardHandler>()),

                Component.For<OptionsViewModel>()
            );
        }

        private void RegisterVideoOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Component.For<VideoOptionsViewModel>()
                    .Forward<IRequestHandler<SetDisplayModeRequest, Unit>>()
                    .Forward<IRequestHandler<SaveVideoOptionsRequest, Unit>>()
                    .Forward<IRequestHandler<VideoOptionsFullScreenToggle, Unit>>()
                    .Forward<IRequestHandler<VideoOptionsVerticalSyncToggle, Unit>>()
                    .ImplementedBy<VideoOptionsViewModel>(),

                Component.For<VideoOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, VideoOptionsKeyboardHandler>())
            );
        }

        private void RegisterGameView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<GameView>()
                    .Forward<IRequestHandler<OpenInGameOptionsRequest, Unit>>()
                    .Forward<IRequestHandler<CloseInGameOptionsRequest, Unit>>()
                    .Forward<IRequestHandler<OpenConsoleRequest, Unit>>()
                    .Forward<IRequestHandler<CloseConsoleRequest, Unit>>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, GameViewKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<IMouseHandler, GameViewMouseHandler>()),

                Component.For<GameViewModel>(),

                Component.For<ICameraMovement>()
                    .ImplementedBy<StrategyCameraMovement>(),

                Component.For<ICameraMovement>()
                    .ImplementedBy<FreeCameraMovement>()
            );
        }

        private void RegisterInGameOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<InGameOptionsView>()
                    .ImplementedBy<InGameOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, InGameOptionsKeyboardHandler>()),

                Component.For<InGameOptionsViewModel>()
            );
        }

        private void RegisterConsoleView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ConsoleView>()
                    .ImplementedBy<ConsoleView>()
                    .Forward<IRequestHandler<UpdateViewRequest<ConsoleData>, Unit>>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, ConsoleKeyboardHandler>()),

                Component.For<IKeyboardHandler>()
                    .ImplementedBy<ConsoleKeyboardHandler>(),

                Component.For<ConsoleViewModel>()
                    .Forward<IRequestHandler<RecallConsoleHistoryBackRequest, Unit>>()
                    .Forward<IRequestHandler<RecallConsoleHistoryForwardRequest, Unit>>()
                    .Forward<IRequestHandler<ExecuteConsoleCommandRequest, Unit>>(),

                Classes.FromThisAssembly()
                    .BasedOn<IConsoleCommand>()
                    .WithServiceDefaultInterfaces()
            );
        }

        private void RegisterGameSpeedView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<GameSpeedView>()
                    .Forward<IRequestHandler<UpdateViewRequest<GameSpeedData>, Unit>>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, GameSpeedKeyboardHandler>()),

                Component.For<GameSpeedViewModel>()
                    .Forward<INotificationHandler<GameTimeUpdateNotification>>()
                    .Forward<IRequestHandler<ChangeGameSpeedRequest, Unit>>()
            );
        }
    }
}
