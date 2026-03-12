using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.View.Installers;
using MarsUndiscovered.Game.Installers;
using MarsUndiscovered.Installers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MarsUndiscovered.Tests
{
    [TestClass]
    public abstract class BaseIntegrationTest : BaseTest
    {
        protected IServiceProvider Container;
        protected ServiceCollection Services;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            Services = new ServiceCollection();

            new CoreInstaller().Install(Services);
            new GameInstaller().Install(Services);
            new ViewInstaller().Install(Services);
            new MarsUndiscoveredInstaller().Install(Services);

            Services.AddSingleton<ILogger>(FakeLogger);
            Container = Services.BuildServiceProvider();
        }
    }
}
