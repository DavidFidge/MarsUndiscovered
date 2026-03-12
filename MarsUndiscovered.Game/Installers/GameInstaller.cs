using System.Reflection;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.DependencyInjection;
using MarsUndiscovered.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarsUndiscovered.Game.Installers
{
    public class GameInstaller
    {
        public void Install(IServiceCollection services)
        {
            RegisterFactories(services);

            services.AddTransientWithProperties<IHttpClient, HttpClientWrapper>();
            services.AddTransientWithProperties<IMapGenerator, MapGenerator>();
            services.AddTransientWithProperties<IPrefabProvider, PrefabProvider>();
            services.AddTransientWithProperties<ILevelGenerator, LevelGenerator>();
            services.AddTransientWithProperties<IMonsterGenerator, MonsterGenerator>();
            services.AddTransientWithProperties<IItemGenerator, ItemGenerator>();
            services.AddTransientWithProperties<IFeatureGenerator, FeatureGenerator>();
            services.AddTransientWithProperties<IEnvironmentalEffectGenerator, EnvironmentalEffectGenerator>();
            services.AddTransientWithProperties<IWaypointGenerator, WaypointGenerator>();
            services.AddTransientWithProperties<IMachineGenerator, MachineGenerator>();
            services.AddTransientWithProperties<IShipGenerator, ShipGenerator>();
            services.AddTransientWithProperties<IMapExitGenerator, MapExitGenerator>();
            services.AddTransientWithProperties<ISpawner, Spawner>();
            services.AddSingletonWithProperties<IGameWorldProvider, GameWorldProvider>();
            services.AddTransientWithProperties<IGameWorld, GameWorld>();
            services.AddTransientWithProperties<IMorgue, Morgue>();
            services.AddTransientWithProperties<IStory, Story>();
            services.AddTransientWithProperties<IMorgueWebService, MorgueWebService>();
            services.AddTransientWithProperties<IMorgueFileWriter, MorgueFileWriter>();
            services.AddTransientWithProperties<IWaveFunctionCollapseGeneratorPasses, WaveFunctionCollapseGeneratorPasses>();
            services.AddTransientWithProperties<IWaveFunctionCollapseGeneratorPassesRenderer, GameWorldWaveFunctionCollapseGeneratorPassesRenderer>();
            services.AddTransientWithProperties<IWaveFunctionCollapseGeneratorPassesContentLoader, GameWorldWaveFunctionCollapseGeneratorPassesContentLoader>();

            AddTransientSelfAndInterface<IGameCamera>(services, typeof(IGameCamera).Assembly);
            AddTransientSelfAndInterface<MarsGameObject>(services, Assembly.GetExecutingAssembly());
            AddTransientSelfAndInterface<BaseGameActionCommand>(services, Assembly.GetExecutingAssembly());
        }

        private void RegisterFactories(IServiceCollection services)
        {
            services.AddTransientWithProperties<IGameObjectFactory, GameObjectFactory>();
            services.AddTransient(typeof(IFactory<IGameWorld>), typeof(ServiceFactory<IGameWorld>));
            services.AddTransient<ICommandCollection>(sp => sp.CreateWithInjectedProperties<CommandCollection>());

            services.AddTransient(typeof(ICommandFactory<MoveCommand>), typeof(CommandFactory<MoveCommand>));
            services.AddTransient(typeof(ICommandFactory<WalkCommand>), typeof(CommandFactory<WalkCommand>));
            services.AddTransient(typeof(ICommandFactory<MeleeAttackCommand>), typeof(CommandFactory<MeleeAttackCommand>));
            services.AddTransient(typeof(ICommandFactory<LineAttackCommand>), typeof(CommandFactory<LineAttackCommand>));
            services.AddTransient(typeof(ICommandFactory<LightningAttackCommand>), typeof(CommandFactory<LightningAttackCommand>));
            services.AddTransient(typeof(ICommandFactory<LaserAttackCommand>), typeof(CommandFactory<LaserAttackCommand>));
            services.AddTransient(typeof(ICommandFactory<DeathCommand>), typeof(CommandFactory<DeathCommand>));
            services.AddTransient(typeof(ICommandFactory<PickUpItemCommand>), typeof(CommandFactory<PickUpItemCommand>));
            services.AddTransient(typeof(ICommandFactory<DropItemCommand>), typeof(CommandFactory<DropItemCommand>));
            services.AddTransient(typeof(ICommandFactory<EquipItemCommand>), typeof(CommandFactory<EquipItemCommand>));
            services.AddTransient(typeof(ICommandFactory<UnequipItemCommand>), typeof(CommandFactory<UnequipItemCommand>));
            services.AddTransient(typeof(ICommandFactory<ChangeMapCommand>), typeof(CommandFactory<ChangeMapCommand>));
            services.AddTransient(typeof(ICommandFactory<ApplyItemCommand>), typeof(CommandFactory<ApplyItemCommand>));
            services.AddTransient(typeof(ICommandFactory<ApplyShieldCommand>), typeof(CommandFactory<ApplyShieldCommand>));
            services.AddTransient(typeof(ICommandFactory<ApplyHealingBotsCommand>), typeof(CommandFactory<ApplyHealingBotsCommand>));
            services.AddTransient(typeof(ICommandFactory<EnchantItemCommand>), typeof(CommandFactory<EnchantItemCommand>));
            services.AddTransient(typeof(ICommandFactory<WaitCommand>), typeof(CommandFactory<WaitCommand>));
            services.AddTransient(typeof(ICommandFactory<ApplyMachineCommand>), typeof(CommandFactory<ApplyMachineCommand>));
            services.AddTransient(typeof(ICommandFactory<IdentifyItemCommand>), typeof(CommandFactory<IdentifyItemCommand>));
            services.AddTransient(typeof(ICommandFactory<UndoCommand>), typeof(CommandFactory<UndoCommand>));
            services.AddTransient(typeof(ICommandFactory<ApplyForcePushCommand>), typeof(CommandFactory<ApplyForcePushCommand>));
            services.AddTransient(typeof(ICommandFactory<PlayerRangeAttackCommand>), typeof(CommandFactory<PlayerRangeAttackCommand>));
            services.AddTransient(typeof(ICommandFactory<ExplodeTileCommand>), typeof(CommandFactory<ExplodeTileCommand>));
            services.AddTransient(typeof(ICommandFactory<SwapPositionCommand>), typeof(CommandFactory<SwapPositionCommand>));
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
