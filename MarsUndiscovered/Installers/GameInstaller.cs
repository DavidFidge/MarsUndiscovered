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
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Installers;
using FrigidRogue.MonoGame.Core.View.Interfaces;

using InputHandlers.Keyboard;
using InputHandlers.Mouse;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;

namespace MarsUndiscovered.Installers
{
    public class GameInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(new AutoMapperProfileInstaller());
            container.Install(new CoreInstaller());
            container.Install(new ViewInstaller());

            RegisterTitleView(container, store);
            RegisterCustomGameSeedView(container, store);
            RegisterLoadGameView(container, store);
            RegisterLoadReplayView(container, store);
            RegisterOptionsView(container, store);
            RegisterVideoOptionsView(container, store);
            RegisterInGameOptionsView(container, store);
            RegisterInReplayOptionsView(container, store);
            RegisterConsoleView(container, store);
            RegisterGameView(container, store);
            RegisterReplayView(container, store);
            RegisterSaveGameView(container, store);

            RegisterKeyboardHandlers(container);
            RegisterMouseHandlers(container);

            RegisterFactories(container);

            container.Register(

                Component.For<IAssets>()
                    .ImplementedBy<Assets>(),

                Component.For<IGame>()
                    .ImplementedBy<MarsUndiscoveredGame>(),

                Component.For<IMapGenerator>()
                    .ImplementedBy<MapGenerator>(),

                Component.For<IMonsterGenerator>()
                    .ImplementedBy<MonsterGenerator>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IScreen>(),

                Component.For<ScreenCollection>(),

                Component.For<GoalMaps>(),

                Component.For<IHeightMapGenerator>()
                    .ImplementedBy<HeightMapGenerator>(),
                
                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .WithServiceDefaultInterfaces(),

                Component.For<IGameWorldProvider>()
                    .ImplementedBy<GameWorldProvider>(),

                Component.For<IGameWorld>()
                    .ImplementedBy<GameWorld>()
                    .LifestyleTransient(),

                Component.For<IActionMapStore>()
                    .ImplementedBy<DefaultActionMapStore>(),


                Component.For<MapViewModel>()
                    .LifeStyle.Transient,

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<MarsGameObject>()
                    .LifestyleTransient(),

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

                Component.For<IGameObjectFactory>()
                    .ImplementedBy<GameObjectFactory>()
                    .DependsOn(Dependency.OnValue<IWindsorContainer>(container)),

                Component.For<MapTileEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<MapTileEntity>>()
                    .AsFactory(),

                Component.For<MapEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<MapEntity>>()
                    .AsFactory(),

                Component.For<MapTileRootEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<MapTileRootEntity>>()
                    .AsFactory(),

                Component.For<GoalMapEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<GoalMapEntity>>()
                    .AsFactory(),

                Component.For<IFactory<IGameWorld>>()
                    .AsFactory(),

                Component.For<ICommandFactory>()
                    .ImplementedBy<CommandFactory>(),

                Component.For<IFactory<MoveCommand>>()
                   .AsFactory(),

                Component.For<IFactory<WalkCommand>>()
                    .AsFactory(),

                Component.For<IFactory<AttackCommand>>()
                    .AsFactory()
            );
        }

        private void RegisterMouseHandlers(IWindsorContainer container)
        {
            container.Register(
                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IMouseHandler>()
                    .ConfigureFor<NullMouseHandler>(c => c.IsDefault())
                    .WithServiceDefaultInterfaces()
            );
        }

        private void RegisterKeyboardHandlers(IWindsorContainer container)
        {
            container.Register(
                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IKeyboardHandler>()
                    .ConfigureFor<NullKeyboardHandler>(c => c.IsDefault())
                    .ConfigureFor<GameViewKeyboardHandler>(c => c.DependsOn(Dependency.OnComponent<ICameraMovement, CameraMovement>()))
                    .WithServiceDefaultInterfaces()
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

        private void RegisterGameView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<GameView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, GameViewKeyboardHandler>())
                    .DependsOn(Dependency.OnComponent<IMouseHandler, GameViewMouseHandler>()),

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

        private void RegisterConsoleView(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ConsoleView>()
                    .DependsOn(Dependency.OnComponent<IKeyboardHandler, ConsoleKeyboardHandler>()),

                Component.For<IKeyboardHandler>()
                    .ImplementedBy<ConsoleKeyboardHandler>(),

                Component.For<ConsoleViewModel>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IConsoleCommand>()
                    .WithServiceDefaultInterfaces()
            );
        }
    }
}
