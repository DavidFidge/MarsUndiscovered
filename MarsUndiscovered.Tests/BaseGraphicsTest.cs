using FrigidRogue.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Tests
{
    [TestClass]
    public abstract class BaseGraphicsTest : BaseTest
    {
        protected GraphicsDevice GraphicsDevice;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

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
