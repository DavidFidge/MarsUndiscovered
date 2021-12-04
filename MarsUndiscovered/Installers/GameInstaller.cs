using System.Reflection;

using MarsUndiscovered.Components;
using MarsUndiscovered.Graphics;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.Messages.Console;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.Input;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;
using MarsUndiscovered.UserInterface.Screens;
using MarsUndiscovered.UserInterface.ViewModels;
using MarsUndiscovered.UserInterface.Views;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Terrain;
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Installers;

using InputHandlers.Keyboard;
using InputHandlers.Mouse;

using MediatR;

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
                    .Forward<IRequestHandler<ExitGameRequest, Unit>>()
                    .ImplementedBy<MarsUndiscoveredGame>(),

                Component.For<IScreenManager>()
                    .Forward<IRequestHandler<NewGameRequest, Unit>>()
                    .Forward<IRequestHandler<ExitCurrentGameRequest, Unit>>()
                    .ImplementedBy<ScreenManager>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<Screen>(),
                
                Component.For<IHeightMapGenerator>()
                    .ImplementedBy<HeightMapGenerator>(),
                
                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .LifestyleTransient()
                    .WithServiceDefaultInterfaces(),

                Component.For<IMarsUndiscoveredGameWorld>()
                    .ImplementedBy<MarsUndiscoveredGameWorld>(),

                Component.For<IActionMapStore>()
                    .ImplementedBy<DefaultActionMapStore>()
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
                    .ImplementedBy<StrategyCameraMovement>()
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

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<IConsoleCommand>()
                    .WithServiceDefaultInterfaces()
            );
        }
    }
}
