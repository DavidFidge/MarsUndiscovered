using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.View.Installers;
using MarsUndiscovered.Game.Installers;
using MarsUndiscovered.Installers;
using Serilog;

namespace MarsUndiscovered.Tests
{
    [TestClass]
    public abstract class BaseIntegrationTest : BaseTest
    {
        protected WindsorContainer Container;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            
            Container = new WindsorContainer();

            Container.Install(new CoreInstaller());
            Container.Install(new GameInstaller());
            Container.Install(new ViewInstaller());
            Container.Install(new MarsUndiscoveredInstaller());

            Container.Register(Component.For<ILogger>().Instance(FakeLogger).IsDefault());
        }
    }
}
