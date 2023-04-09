using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.View.Installers;
using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Game.Installers;
using MarsUndiscovered.Installers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace MarsUndiscovered.Tests
{
    [TestClass]
    public abstract class BaseIntegrationTest
    {
        protected WindsorContainer Container;
        protected FakeLogger FakeLogger;

        [TestInitialize]
        public virtual void Setup()
        {
            Container = new WindsorContainer();

            Container.Install(new CoreInstaller());
            Container.Install(new GameInstaller());
            Container.Install(new ViewInstaller());
            Container.Install(new MarsUndiscoveredInstaller());

            FakeLogger = new FakeLogger();
            Container.Register(Component.For<ILogger>().Instance(FakeLogger).IsDefault());
        }

        [TestCleanup]
        public virtual void TearDown()
        {
        }
    }
}
