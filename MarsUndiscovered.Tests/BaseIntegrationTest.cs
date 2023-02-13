using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FrigidRogue.TestInfrastructure;
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

            Container.Install(new GameInstaller());

            FakeLogger = new FakeLogger();
            Container.Register(Component.For<ILogger>().Instance(FakeLogger).IsDefault());
        }

        [TestCleanup]
        public virtual void TearDown()
        {
        }
    }
}
