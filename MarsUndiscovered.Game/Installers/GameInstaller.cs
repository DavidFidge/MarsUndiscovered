using System.Reflection;

using Castle.Facilities.TypedFactory;

using MarsUndiscovered.Interfaces;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

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

namespace MarsUndiscovered.Game.Installers
{
    public class GameInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterFactories(container);

            container.Register(
               
                Component.For<IHttpClient>()
                    .ImplementedBy<HttpClientWrapper>(),

                Component.For<IMapGenerator>()
                    .ImplementedBy<MapGenerator>()
                    .LifestyleTransient(),

                Component.For<ILevelGenerator>()
                    .ImplementedBy<LevelGenerator>()
                    .LifestyleTransient(),

                Component.For<IMonsterGenerator>()
                    .ImplementedBy<MonsterGenerator>()
                    .LifestyleTransient(),

                Component.For<IItemGenerator>()
                    .ImplementedBy<ItemGenerator>()
                    .LifestyleTransient(),

                Component.For<IMachineGenerator>()
                    .ImplementedBy<MachineGenerator>()
                    .LifestyleTransient(),

                Component.For<IShipGenerator>()
                    .ImplementedBy<ShipGenerator>()
                    .LifestyleTransient(),
                
                Component.For<IMiningFacilityGenerator>()
                    .ImplementedBy<MiningFacilityGenerator>()
                    .LifestyleTransient(),

                Component.For<IMapExitGenerator>()
                    .ImplementedBy<MapExitGenerator>()
                    .LifestyleTransient(),

                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .WithServiceDefaultInterfaces(),

                Component.For<IGameWorldEndpoint, IGameWorldConsoleCommandEndpoint>()
                    .ImplementedBy<GameWorldEndpoint>(),

                Component.For<IGameWorldDebug>()
                    .ImplementedBy<GameWorldDebug>()
                    .LifestyleTransient(),

                Component.For<IGameWorld>()
                    .ImplementedBy<GameWorld>()
                    .LifestyleTransient(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<MarsGameObject>()
                    .LifestyleTransient(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<BaseGameActionCommand>()
                    .LifestyleTransient(),
                
                Component.For<IMorgue>()
                    .ImplementedBy<Morgue>()
                    .LifestyleTransient(),
                
                Component.For<IMorgueWebService>()
                    .ImplementedBy<MorgueWebService>(),
                
                Component.For<IMorgueFileWriter>()
                    .ImplementedBy<MorgueFileWriter>()
                    .LifestyleTransient(),

                Component.For<IWaveFunctionCollapseGeneratorPasses>()
                    .ImplementedBy<WaveFunctionCollapseGeneratorPasses>(),

                Component.For<IWaveFunctionCollapseGeneratorPassesRenderer>()
                    .ImplementedBy<GameWorldWaveFunctionCollapseGeneratorPassesRenderer>(),

                Component.For<IWaveFunctionCollapseGeneratorPassesContentLoader>()
                    .ImplementedBy<GameWorldWaveFunctionCollapseGeneratorPassesContentLoader>()
            );
        }

        private void RegisterFactories(IWindsorContainer container)
        {
            container.Register(

                Component.For<IGameObjectFactory>()
                    .ImplementedBy<GameObjectFactory>()
                    .DependsOn(Dependency.OnValue<IWindsorContainer>(container))
                    .LifeStyle.Transient,

                Component.For<IFactory<IGameWorld>>()
                    .AsFactory(),

                Component.For<ICommandFactory>()
                    .ImplementedBy<CommandFactory>(),

                Component.For<ICommandFactory<MoveCommand>>()
                   .AsFactory(),

                Component.For<ICommandFactory<WalkCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<MeleeAttackCommand>>()
                    .AsFactory(),
                
                Component.For<ICommandFactory<LineAttackCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<LightningAttackCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<DeathCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<PickUpItemCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<DropItemCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<EquipItemCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<UnequipItemCommand>>()
                    .AsFactory(),

                Component.For<ICommandFactory<ChangeMapCommand>>()
                    .AsFactory(),
                
                Component.For<ICommandFactory<ApplyItemCommand>>()
                    .AsFactory(),
                
                Component.For<ICommandFactory<ApplyShieldCommand>>()
                    .AsFactory(),
                
                Component.For<ICommandFactory<ApplyHealingBotsCommand>>()
                    .AsFactory(),
                
                Component.For<ICommandFactory<EnchantItemCommand>>()
                    .AsFactory(),
                
                Component.For<ICommandFactory<WaitCommand>>()
                    .AsFactory()
            );
        }
    }
}
