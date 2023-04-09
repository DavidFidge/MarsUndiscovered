using System.Diagnostics;
using System.Reflection;

using Castle.Facilities.TypedFactory;

using MarsUndiscovered.Components;
using MarsUndiscovered.Graphics;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Input;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;
using MarsUndiscovered.UserInterface.ViewModels;
using MarsUndiscovered.UserInterface.Views;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Terrain;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Interfaces;

using InputHandlers.Keyboard;
using InputHandlers.Mouse;

using Microsoft.Extensions.Configuration;

namespace MarsUndiscovered.Installers
{
    public class MarsUndiscoveredInstaller : IWindsorInstaller
    {
        [Conditional("DEBUG")]
        private void SetDebugEnvironment(ref string environment)
        {
            environment = "Development";
        }
        
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var environment = "Production";
            
            SetDebugEnvironment(ref environment);
            
            var configuration =  new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();
            
            container.Register(Component.For<IConfiguration>().Instance(configuration));

            RegisterTitleView(container, store);
            RegisterCustomGameSeedView(container, store);
            RegisterLoadGameView(container, store);
            RegisterLoadReplayView(container, store);
            RegisterOptionsView(container, store);
            RegisterVideoOptionsView(container, store);
            RegisterGameOptionsView(container, store);
            RegisterInGameOptionsView(container, store);
            RegisterInReplayOptionsView(container, store);
            RegisterWorldBuilderOptionsView(container, store);
            RegisterConsoleView(container, store);
            RegisterGameView(container, store);
            RegisterReplayView(container, store);
            RegisterWorldBuilderView(container, store);
            RegisterSaveGameView(container, store);
            RegisterInventoryGameView(container, store);
            RegisterInventoryReplayView(container, store);

            RegisterKeyboardHandlers(container);
            RegisterMouseHandlers(container);

            RegisterFactories(container);

            container.Register(

                Component.For<IAssets>()
                    .ImplementedBy<Assets>(),

                Component.For<IGame>()
                    .ImplementedBy<MarsUndiscoveredGame>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IScreen>(),

                Component.For<ScreenCollection>(),

                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .WithServiceDefaultInterfaces(),

                Component.For<IActionMapStore>()
                    .ImplementedBy<DefaultActionMapStore>(),

                Component.For<MapViewModel>()
                    .LifeStyle.Transient,

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<BaseGameActionCommand>()
                    .LifestyleTransient(),

                Component.For<ICameraMovement>()
                    .ImplementedBy<CameraMovement>()
            );
        }

        private void RegisterFactories(IWindsorContainer container)
        {
            container.Register(

                Component.For<MapTileEntity>()
                    .LifeStyle.Transient,

                Component.For<IMapTileEntityFactory>()
                    .AsFactory(),

                Component.For<FieldOfViewTileEntity>()
                    .LifeStyle.Transient,

                Component.For<IFieldOfViewTileEntityFactory>()
                    .AsFactory(),

                Component.For<MapEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<MapEntity>>()
                    .AsFactory(),

                Component.For<GoalMapEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<GoalMapEntity>>()
                    .AsFactory()
            );
        }

        private void RegisterMouseHandlers(IWindsorContainer container)
        {
            container.Register(
                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IMouseHandler>() // Only covers dependencies asking for IMouseHandler, does not cover if they ask for specific class type e.g. see RegisterGameView
                    .ConfigureFor<NullMouseHandler>(c => c.IsDefault())
                    .WithServiceDefaultInterfaces()
            );
        }

        private void RegisterKeyboardHandlers(IWindsorContainer container)
        {
            
            container.Register(
                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IKeyboardHandler>() // Only covers dependencies asking for IKeyboardHandler, does not cover if they ask for specific class type e.g. see RegisterGameView
                    .Unless(s => typeof(GlobalKeyboardHandler).IsAssignableFrom(s))
                    .ConfigureFor<NullKeyboardHandler>(c => c.IsDefault())
                    .ConfigureFor<GameViewKeyboardHandler>(c => c.DependsOn(Dependency.OnComponent<ICameraMovement, CameraMovement>()))
                    .WithServiceDefaultInterfaces(),

                Component.For<GlobalKeyboardHandler>()
            );
        }

        private void RegisterTitleView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<TitleView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, TitleViewKeyboardHandler>()),

                Component.For<TitleViewModel>()
            );
        }

        private void RegisterLoadGameView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<LoadGameView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, LoadGameViewKeyboardHandler>()),

                Component.For<LoadGameViewModel>()
            );
        }

        private void RegisterLoadReplayView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<LoadReplayView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, LoadReplayViewKeyboardHandler>()),

                Component.For<LoadReplayViewModel>()
            );
        }

        private void RegisterCustomGameSeedView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<CustomGameSeedView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, CustomGameSeedViewKeyboardHandler>()),

                Component.For<CustomGameSeedViewModel>()
            );
        }

        private void RegisterSaveGameView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<SaveGameView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, SaveGameViewKeyboardHandler>()),

                Component.For<SaveGameViewModel>()
            );
        }

        private void RegisterOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<OptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, OptionsKeyboardHandler>()),

                Component.For<OptionsViewModel>()
            );
        }

        private void RegisterVideoOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Component.For<VideoOptionsViewModel>()
                    .ImplementedBy<VideoOptionsViewModel>(),

                Component.For<VideoOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, VideoOptionsKeyboardHandler>())
            );
        }
        
        private void RegisterGameOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Component.For<GameOptionsViewModel>()
                    .ImplementedBy<GameOptionsViewModel>(),

                Component.For<GameOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, GameOptionsKeyboardHandler>())
            );
        }

        private void RegisterGameView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<GameView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, GameViewKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<IMouseHandler, GameViewMouseHandler>())
                    .DependsOn(Dependency.OnComponent<GameViewRadioCommsMouseHandler, GameViewRadioCommsMouseHandler>())
                    .DependsOn(Dependency.OnComponent<GameViewRadioCommsKeyboardHandler, GameViewRadioCommsKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<GameViewGameOverMouseHandler, GameViewGameOverMouseHandler>())
                    .DependsOn(Dependency.OnComponent<GameViewGameOverKeyboardHandler, GameViewGameOverKeyboardHandler>()),

                Component.For<GameViewModel>()
            );
        }

        private void RegisterReplayView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ReplayView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, ReplayViewKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<IMouseHandler, ReplayViewMouseHandler>()),

                Component.For<ReplayViewModel>()
            );
        }
        
        private void RegisterWorldBuilderView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<WorldBuilderView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, WorldBuilderViewKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<IMouseHandler, WorldBuilderViewMouseHandler>()),

                Component.For<WorldBuilderViewModel>()
            );
        }

        private void RegisterInGameOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<InGameOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, InGameOptionsKeyboardHandler>()),

                Component.For<InGameOptionsViewModel>()
            );
        }

        private void RegisterInReplayOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<InReplayOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, InReplayOptionsKeyboardHandler>()),

                Component.For<InReplayOptionsViewModel>()
            );
        }
        
        private void RegisterWorldBuilderOptionsView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<WorldBuilderOptionsView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, WorldBuilderOptionsKeyboardHandler>()),

                Component.For<WorldBuilderOptionsViewModel>()
            );
        }

        private void RegisterConsoleView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ConsoleView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, ConsoleKeyboardHandler>()),

                Component.For<ConsoleViewModel>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IConsoleCommand>()
                    .WithServiceDefaultInterfaces()
            );
        }

        private void RegisterInventoryGameView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<InventoryGameView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, InventoryGameViewKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<IMouseHandler, NullMouseHandler>()),

                Component.For<InventoryGameViewModel>()
            );
        }

        private void RegisterInventoryReplayView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<InventoryReplayView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, InventoryReplayViewKeyboardHandler>()),

                Component.For<InventoryReplayViewModel>()
            );
        }
    }
}
