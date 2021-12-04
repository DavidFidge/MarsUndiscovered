using System;

using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Installers;

namespace MarsUndiscovered
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var container = new WindsorContainer();

            container.Install(new GameInstaller());

            using var game = container.Resolve<IGame>();

            game.Run();
        }
    }
}
