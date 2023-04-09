using System.Diagnostics;
using System.Reflection;

using Castle.Facilities.TypedFactory;

using MarsUndiscovered.Interfaces;

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

using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using Microsoft.Extensions.Configuration;

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
                    .ImplementedBy<MapGenerator>(),

                Component.For<IMonsterGenerator>()
                    .ImplementedBy<MonsterGenerator>(),

                Component.For<IItemGenerator>()
                    .ImplementedBy<ItemGenerator>(),

                Component.For<IShipGenerator>()
                    .ImplementedBy<ShipGenerator>(),
                
                Component.For<IMiningFacilityGenerator>()
                    .ImplementedBy<MiningFacilityGenerator>(),

                Component.For<IMapExitGenerator>()
                    .ImplementedBy<MapExitGenerator>(),

                Component.For<IHeightMapGenerator>()
                    .ImplementedBy<HeightMapGenerator>(),
                
                Classes.FromAssemblyContaining<IGameCamera>()
                    .BasedOn<IGameCamera>()
                    .WithServiceDefaultInterfaces(),

                Component.For<IGameWorldEndpoint, IGameWorldConsoleCommandEndpoint>()
                    .ImplementedBy<GameWorldEndpoint>(),

                Component.For<IGameWorld>()
                    .ImplementedBy<GameWorld>()
                    .LifestyleTransient(),

                Component.For<IActionMapStore>()
                    .ImplementedBy<DefaultActionMapStore>(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<MarsGameObject>()
                    .LifestyleTransient(),

                Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<BaseGameActionCommand>()
                    .LifestyleTransient(),
                
                Component.For<IMorgue>()
                    .ImplementedBy<Morgue>(),
                
                Component.For<IMorgueWebService>()
                    .ImplementedBy<MorgueWebService>(),
                
                Component.For<IMorgueFileWriter>()
                    .ImplementedBy<MorgueFileWriter>()
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
                    .AsFactory()
            );
        }
    }
}
