using System.IO;
using System.Text;

using GoRogue.FOV;
using GoRogue.Pathing;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

using Serilog;
using Serilog.Events;
using Color = Microsoft.Xna.Framework.Color;

namespace MarsUndiscovered.Game.Components
{
    public static class DebugWriter
    {
        public static void DumpFieldOfViewToFile(IFOV fieldOfView)
        {
            var stringBuilder = DumpFieldOfView(fieldOfView);
            
            WriteToTempFile("FieldOfView", stringBuilder);
        }

        public static void DumpFieldOfViewToLog(IFOV fieldOfView, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Current of view:");

            var stringBuilder = DumpFieldOfView(fieldOfView);
            
            WriteToLog(logger, stringBuilder);
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
            var stringBuilder = DumpGoalMap(goalMap);

            WriteToTempFile("GoalMap", stringBuilder);
        }

        private static void WriteToTempFile(string filename, StringBuilder stringBuilder)
        {
            var fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss-FFFFFF")}-{filename}.txt");

            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public static void DumpGoalMapToLog(GoalMap goalMap, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Goal Map:");

            var stringBuilder = DumpGoalMap(goalMap);

            WriteToLog(logger, stringBuilder);
        }

        private static void WriteToLog(ILogger logger, StringBuilder stringBuilder)
        {
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
            var stringBuilder = DumpMap(map);
            
            WriteToTempFile("Map", stringBuilder);
        }

        public static void DumpMapToLog(MarsMap map, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug($"Map Level: {map.Level} | Id: {map.Id}");

            var stringBuilder = DumpMap(map);
            
            WriteToLog(logger, stringBuilder);
        }

        public static StringBuilder DumpTexture(Texture2D texture, Dictionary<Color, char> colourToChar)
        {
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < texture.Height; y++)
            {
                for (var x = 0; x < texture.Width; x++)
                {
                    if (colourToChar.ContainsKey(data[x + y * texture.Width]))
                        stringBuilder.Append(colourToChar[data[x + y * texture.Width]]);
                    else
                        stringBuilder.Append('.');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }
        
        public static void DumpTextureToFile(Texture2D texture, Dictionary<Color, char> colourToChar)
        {
            var stringBuilder = DumpTexture(texture, colourToChar);
            
            WriteToTempFile("Texture", stringBuilder);
        }
        
        public static void DumpTextureToLog(Texture2D texture, Dictionary<Color, char> colourToChar, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Texture:");

            var stringBuilder = DumpTexture(texture, colourToChar);
            
            WriteToLog(logger, stringBuilder);
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
