using System;

using MarsUndiscovered.Installers;

using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var container = new WindsorContainer();

            container.Install(new GameInstaller());

            using (var game = container.Resolve<IGame>())
                game.Run();
        }
    }
#endif
}
