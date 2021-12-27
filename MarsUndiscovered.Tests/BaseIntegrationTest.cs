using Castle.Windsor;
using MarsUndiscovered.Installers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests
{
    [TestClass]
    public abstract class BaseIntegrationTest
    {
        protected WindsorContainer Container;

        [TestInitialize]
        public virtual void Setup()
        {
            Container = new WindsorContainer();

            Container.Install(new GameInstaller());
        }

        [TestCleanup]
        public virtual void TearDown()
        {
        }
    }
}
