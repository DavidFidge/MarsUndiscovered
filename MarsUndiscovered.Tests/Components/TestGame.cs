using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.Services;
using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.Tests.Components;

public class TestGame : IGame
{
    public Task<Unit> Handle(QuitToDesktopRequest request, CancellationToken cancellationToken)
    {
        return Unit.Task;
    }

    public Task<Unit> Handle(ToggleFullScreenRequest request, CancellationToken cancellationToken)
    {
        return Unit.Task;
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