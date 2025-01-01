using System.Threading;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Tests.Components;

public class TestGame : IGame
{
    public void Handle(QuitToDesktopRequest request)
    {
    }

    public void Handle(ToggleFullScreenRequest request)
    {
    }

    public void Dispose()
    {
        Content.Dispose();
        GraphicsDevice.Dispose();
    }

    public GameComponentCollection Components { get; set;}
    public GameServiceContainer Services { get; set;}
    public ContentManager Content { get; set; }
    public GraphicsDevice GraphicsDevice { get; set;}
    public GameWindow Window { get; set; }
    public CustomGraphicsDeviceManager CustomGraphicsDeviceManager { get; set;}
    public void Run()
    {
    }

    public EffectCollection EffectCollection { get; set; }
}