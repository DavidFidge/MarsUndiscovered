using System.Diagnostics;
using System.Reflection;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Map;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Interfaces;
using InputHandlers.Keyboard;
using InputHandlers.Mouse;
using MarsUndiscovered.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.DependencyInjection;
using MarsUndiscovered.Graphics;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Input;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;
using MarsUndiscovered.UserInterface.ViewModels;
using MarsUndiscovered.UserInterface.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarsUndiscovered.Installers
{
    public class MarsUndiscoveredInstaller
    {
        [Conditional("DEBUG")]
        private void SetDebugEnvironment(ref string environment)
        {
            environment = "Development";
        }

        public void Install(IServiceCollection services)
        {
            var environment = "Production";

            SetDebugEnvironment(ref environment);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            RegisterViews(services);
            RegisterHandlers(services);
            RegisterFactories(services);

            services.AddTransientWithProperties<IAssets, Assets>();
            services.AddTransientWithProperties<IGame, MarsUndiscoveredGame>();
            services.AddTransientWithProperties<ScreenCollection, ScreenCollection>();
            services.AddTransientWithProperties<IActionMapStore, DefaultActionMapStore>();
            services.AddTransient<MapViewModel>(sp => sp.CreateWithInjectedProperties<MapViewModel>());
            services.AddTransientWithProperties<ICameraMovement, CameraMovement>();

            AddTransientSelfAndInterface<IScreen>(services, Assembly.GetExecutingAssembly());
            AddTransientSelfAndInterface<IGameCamera>(services, typeof(IGameCamera).Assembly);
            AddTransientSelfAndInterface<BaseGameActionCommand>(services, Assembly.GetExecutingAssembly());
            AddTransientSelfAndInterface<IConsoleCommand>(services, Assembly.GetExecutingAssembly());
        }

        private void RegisterFactories(IServiceCollection services)
        {
            services.AddTransient<MapTileEntity>(sp => sp.CreateWithInjectedProperties<MapTileEntity>());
            services.AddTransient<IMapTileEntityFactory, MapTileEntityFactory>();

            services.AddTransient<FieldOfViewTileEntity>(sp => sp.CreateWithInjectedProperties<FieldOfViewTileEntity>());
            services.AddTransient<IFieldOfViewTileEntityFactory, FieldOfViewTileEntityFactory>();

            services.AddTransient<MapEntity>(sp => sp.CreateWithInjectedProperties<MapEntity>());
            services.AddTransient<IFactory<MapEntity>, ServiceFactory<MapEntity>>();

            services.AddTransient<GoalMapEntity>(sp => sp.CreateWithInjectedProperties<GoalMapEntity>());
            services.AddTransient<IFactory<GoalMapEntity>, ServiceFactory<GoalMapEntity>>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<TitleView>(sp => sp.CreateWithInjectedProperties<TitleView>());
            services.AddTransient<TitleViewModel>(sp => sp.CreateWithInjectedProperties<TitleViewModel>());

            services.AddTransient<LoadGameView>(sp => sp.CreateWithInjectedProperties<LoadGameView>());
            services.AddTransient<LoadGameViewModel>(sp => sp.CreateWithInjectedProperties<LoadGameViewModel>());

            services.AddTransient<CustomGameSeedView>(sp => sp.CreateWithInjectedProperties<CustomGameSeedView>());
            services.AddTransient<CustomGameSeedViewModel>(sp => sp.CreateWithInjectedProperties<CustomGameSeedViewModel>());

            services.AddTransient<SaveGameView>(sp => sp.CreateWithInjectedProperties<SaveGameView>());
            services.AddTransient<SaveGameViewModel>(sp => sp.CreateWithInjectedProperties<SaveGameViewModel>());

            services.AddTransient<OptionsView>(sp => sp.CreateWithInjectedProperties<OptionsView>());
            services.AddTransient<OptionsViewModel>(sp => sp.CreateWithInjectedProperties<OptionsViewModel>());

            services.AddTransient<VideoOptionsView>(sp => sp.CreateWithInjectedProperties<VideoOptionsView>());
            services.AddTransient<VideoOptionsViewModel>(sp => sp.CreateWithInjectedProperties<VideoOptionsViewModel>());

            services.AddTransient<GameOptionsView>(sp => sp.CreateWithInjectedProperties<GameOptionsView>());
            services.AddTransient<GameOptionsViewModel>(sp => sp.CreateWithInjectedProperties<GameOptionsViewModel>());

            services.AddTransient<DeveloperToolsView>(sp => sp.CreateWithInjectedProperties<DeveloperToolsView>());
            services.AddTransient<DeveloperToolsViewModel>(sp => sp.CreateWithInjectedProperties<DeveloperToolsViewModel>());

            services.AddTransient<GameView>(sp => sp.CreateWithInjectedProperties<GameView>());
            services.AddTransient<GameViewModel>(sp => sp.CreateWithInjectedProperties<GameViewModel>());

            services.AddTransient<WorldBuilderView>(sp => sp.CreateWithInjectedProperties<WorldBuilderView>());
            services.AddTransient<WorldBuilderViewModel>(sp => sp.CreateWithInjectedProperties<WorldBuilderViewModel>());

            services.AddTransient<InGameOptionsView>(sp => sp.CreateWithInjectedProperties<InGameOptionsView>());
            services.AddTransient<InGameOptionsViewModel>(sp => sp.CreateWithInjectedProperties<InGameOptionsViewModel>());

            services.AddTransient<InReplayOptionsView>(sp => sp.CreateWithInjectedProperties<InReplayOptionsView>());
            services.AddTransient<InReplayOptionsViewModel>(sp => sp.CreateWithInjectedProperties<InReplayOptionsViewModel>());

            services.AddTransient<WorldBuilderOptionsView>(sp => sp.CreateWithInjectedProperties<WorldBuilderOptionsView>());
            services.AddTransient<WorldBuilderOptionsViewModel>(sp => sp.CreateWithInjectedProperties<WorldBuilderOptionsViewModel>());

            services.AddTransient<ConsoleView>(sp => sp.CreateWithInjectedProperties<ConsoleView>());
            services.AddTransient<ConsoleViewModel>(sp => sp.CreateWithInjectedProperties<ConsoleViewModel>());

            services.AddTransient<InventoryGameView>(sp => sp.CreateWithInjectedProperties<InventoryGameView>());
            services.AddTransient<InventoryGameViewModel>(sp => sp.CreateWithInjectedProperties<InventoryGameViewModel>());

            services.AddTransient<InventoryReplayView>(sp => sp.CreateWithInjectedProperties<InventoryReplayView>());
            services.AddTransient<InventoryReplayViewModel>(sp => sp.CreateWithInjectedProperties<InventoryReplayViewModel>());
        }

        private static void RegisterHandlers(IServiceCollection services)
        {
            AddTransientSelfAndInterface<IKeyboardHandler>(services, Assembly.GetExecutingAssembly());
            AddTransientSelfAndInterface<IMouseHandler>(services, Assembly.GetExecutingAssembly());

            services.AddTransient<IKeyboardHandler, NullKeyboardHandler>();
            services.AddTransient<IMouseHandler, NullMouseHandler>();
        }

        private static void AddTransientSelfAndInterface<TServiceBase>(IServiceCollection services, Assembly assembly)
        {
            var serviceType = typeof(TServiceBase);
            var implementations = assembly
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && serviceType.IsAssignableFrom(t))
                .ToList();

            foreach (var implementation in implementations)
            {
                services.AddTransient(implementation, sp => sp.CreateWithInjectedProperties(implementation));

                var interfaces = implementation
                    .GetInterfaces()
                    .Where(i => serviceType.IsAssignableFrom(i))
                    .ToList();

                foreach (var @interface in interfaces)
                {
                    services.AddTransient(@interface, sp => sp.GetRequiredService(implementation));
                }
            }
        }
    }
}
