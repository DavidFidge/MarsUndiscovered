using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.View.Installers;
using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Game.Installers;
using MarsUndiscovered.Installers;
using MarsUndiscovered.Tests.Components;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace MarsUndiscovered.Tests
{
    [TestClass]
    public abstract class BaseGraphicsGameWorldIntegrationTests : BaseGameWorldIntegrationTests
    {
        protected GraphicsDevice GraphicsDevice;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            // This only works with SharpDX (i.e. compiling with WindowsDX)
            GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach,
                new PresentationParameters());
        }

        [TestCleanup]
        public override void TearDown()
        {
           base.TearDown();
           GraphicsDevice.Dispose();
        }
    }
}
