using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Tests.Components;

public class TestGraphicsDeviceService : IGraphicsDeviceService
{
    public TestGraphicsDeviceService()
    {
        // This only works with SharpDX (i.e. compiling with WindowsDX)
        GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
            GraphicsProfile.Reach,
            new PresentationParameters());
    }
 
    public event EventHandler<EventArgs> DeviceCreated
    {
        add { }
        remove { }
    }
 
    public event EventHandler<EventArgs> DeviceDisposing
    {
        add { }
        remove { }
    }
 
    public event EventHandler<EventArgs> DeviceReset
    {
        add { }
        remove { }
    }
 
    public event EventHandler<EventArgs> DeviceResetting
    {
        add { }
        remove { }
    }
 
    public GraphicsDevice GraphicsDevice 
    { get; }
}