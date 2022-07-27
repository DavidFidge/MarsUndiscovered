using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommandLine;
using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Installers;

namespace MarsUndiscovered
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunGame);
        }

        static void RunGame(Options options)
        {
            var container = new WindsorContainer();

            container.Install(new GameInstaller());

            container.Register(
                Component.For<Options>()
                    .Instance(options)
            );

            using var game = container.Resolve<IGame>();

            game.Run();
        }
    }
}
