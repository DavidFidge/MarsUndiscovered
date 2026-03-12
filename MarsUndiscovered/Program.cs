using CommandLine;
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.View.Installers;
using MarsUndiscovered.Game.Installers;
using MarsUndiscovered.Installers;
using Microsoft.Extensions.DependencyInjection;

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
            var services = new ServiceCollection();

            new CoreInstaller().Install(services);
            new GameInstaller().Install(services);
            new ViewInstaller().Install(services);
            new MarsUndiscoveredInstaller().Install(services);

            services.AddSingleton(options);

            using var provider = services.BuildServiceProvider();
            using var game = provider.GetRequiredService<IGame>();

            game.Run();
        }
    }
}
