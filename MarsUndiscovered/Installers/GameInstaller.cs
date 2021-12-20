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

namespace MarsUndiscovered.Installers
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

            RegisterKeyboardHandlers(container);
            RegisterMouseHandlers(container);

            container.Register(

                Component.For<IAssets>()
                    .ImplementedBy<Assets>(),

                Component.For<IGame>()
                    .ImplementedBy<MarsUndiscoveredGame>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IScreen>(),

                Component.For<ScreenCollection>(),

                Component.For<IHeightMapGenerator>()
                    .ImplementedBy<HeightMapGenerator>(),
                
                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .WithServiceDefaultInterfaces(),

                Component.For<IGameWorld>()
                    .ImplementedBy<GameWorld>(),

                Component.For<IActionMapStore>()
                    .ImplementedBy<DefaultActionMapStore>(),

                Component.For<MapTileEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<MapTileEntity>>()
                    .AsFactory(),

                Component.For<MapEntity>()
                    .LifeStyle.Transient,

                Component.For<IFactory<MapEntity>>()
                    .AsFactory(),

                Component.For<Player>()
                    .LifestyleTransient(),

                Component.For<Wall>()
                    .LifestyleTransient(),

                Component.For<Floor>()
                    .LifestyleTransient(),

                Component.For<IFactory<Wall>>()
                    .AsFactory(),

                Component.For<IFactory<Floor>>()
                    .AsFactory(),

                Component.For<IFactory<Player>>()
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

                Component.For<GameViewModel>(),

                Component.For<ICameraMovement>()
                    .ImplementedBy<CameraMovement>()
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
