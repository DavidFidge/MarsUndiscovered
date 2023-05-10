using System.IO;
using System.Text;

using GoRogue.FOV;
using GoRogue.Pathing;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

using Serilog;
using Serilog.Events;
using Color = Microsoft.Xna.Framework.Color;
using Path = System.IO.Path;

namespace MarsUndiscovered.Game.Components
{
    public class DumpToFile
    {
        public static void DumpFieldOfViewToFile(IFOV fieldOfView)
        {
            var fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss-FFFFFF")}-FieldOfView.txt");

            var stringBuilder = DumpFieldOfView(fieldOfView);

            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public static void DumpFieldOfViewToLog(IFOV fieldOfView, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Current of view:");

            var stringBuilder = DumpFieldOfView(fieldOfView);

            foreach (var line in stringBuilder.ToString().Split(Environment.NewLine))
            {
                logger.Debug(line);
            }
        }

        private static StringBuilder DumpFieldOfView(IFOV fieldOfView)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < fieldOfView.BooleanResultView.Height; y++)
            {
                for (var x = 0; x < fieldOfView.BooleanResultView.Width; x++)
                {
                    if (fieldOfView.BooleanResultView[x, y])
                        stringBuilder.Append(' ');
                    else
                        stringBuilder.Append('X');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }

        public static void DumpGoalMapToFile(GoalMap goalMap)
        {
            var fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss-FFFFFF")}-GoalMap.txt");

            var stringBuilder = DumpGoalMap(goalMap);

            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public static void DumpGoalMapToLog(GoalMap goalMap, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Goal Map:");

            var stringBuilder = DumpGoalMap(goalMap);

            foreach (var line in stringBuilder.ToString().Split(Environment.NewLine))
            {
                logger.Debug(line);
            }
        }

        private static StringBuilder DumpGoalMap(GoalMap goalMap)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < goalMap.Height; y++)
            {
                for (var x = 0; x < goalMap.Width; x++)
                {
                    if (goalMap[x, y] != null)
                        stringBuilder.Append(Math.Round(goalMap[x, y].Value, 0));
                    else
                        stringBuilder.Append("-");
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }

        public static void DumpMapToFile(MarsMap map)
        {
            var fileName = System.IO.Path.Combine(Path.GetTempPath(), $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss-FFFFFF")}-Map.txt");

            var stringBuilder = DumpMap(map);

            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public static void DumpMapToLog(MarsMap map, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug($"Map Level: {map.Level} | Id: {map.Id}");

            var stringBuilder = DumpMap(map);

            foreach (var line in stringBuilder.ToString().Split(Environment.NewLine))
            {
                logger.Debug(line);
            }
        }

        public static StringBuilder DumpTexture(Texture2D texture, int width, Dictionary<Color, char> colourToChar)
        {

        }

        private static StringBuilder DumpMap(MarsMap map)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var gameObjects = map
                        .GetObjectsAt(x, y)
                        .OrderBy(
                            o =>
                            {
                                switch (o)
                                {
                                    case Indestructible _:
                                        return 0;
                                    case Actor _:
                                        return 1;
                                    case Item _:
                                        return 2;
                                    default:
                                        return 99;
                                }
                            }
                        )
                        .ToList();

                    var firstObject = gameObjects.First();

                    var c = '?';

                    switch (firstObject)
                    {
                        case MapExit mapExit:
                            if (mapExit.Direction == Direction.Down)
                                c = '>';
                            else
                                c = '<';
                            break;
                        case Ship ship:
                            c = ship.ShipPart;
                            break;
                        case Item item:
                            switch (item.ItemType)
                            {
                                case NanoFlask _:
                                    c = '૪';
                                    break;
                                case Gadget _:
                                    c = '⏻';
                                    break;
                                case Weapon _:
                                    c = '↑';
                                    break;
                                case ShipRepairParts _:
                                    c = '&';
                                    break;
                            }

                            break;
                        case Wall _:
                            c = '#';
                            break;
                        case Floor _:
                            c = '.';
                            break;
                        case Player _:
                            c = '@';
                            break;
                        case Monster monster:
                            c = monster.Breed.AsciiCharacter;
                            break;
                    }

                    stringBuilder.Append(c);
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }
    }
}
