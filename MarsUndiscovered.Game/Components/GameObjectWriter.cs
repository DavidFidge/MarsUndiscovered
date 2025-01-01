using System.IO;
using System.Text;
using GoRogue.FOV;
using GoRogue.Pathing;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives.GridViews;
using Serilog;
using Serilog.Events;
using Color = Microsoft.Xna.Framework.Color;
using Path = System.IO.Path;

namespace MarsUndiscovered.Game.Components
{
    public static class GameObjectWriter
    {
        public static void WriteFieldOfViewToFile(IFOV fieldOfView)
        {
            var stringBuilder = WriteFieldOfView(fieldOfView);
            
            WriteToTempFile("FieldOfView", stringBuilder);
        }

        public static void WriteFieldOfViewToLog(IFOV fieldOfView, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Current of view:");

            var stringBuilder = WriteFieldOfView(fieldOfView);
            
            WriteToLog(logger, stringBuilder);
        }

        public static StringBuilder WriteFieldOfView(IFOV fieldOfView)
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

        public static void WriteGoalMapToFile(GoalMap goalMap)
        {
            var stringBuilder = WriteGoalMap(goalMap);

            WriteToTempFile("GoalMap", stringBuilder);
        }

        public static void WriteToTempFile(string filename, StringBuilder stringBuilder)
        {
            var path = Path.Combine(Path.GetTempPath(), filename);

            Directory.CreateDirectory(path);
            
            var fileName = Path.Combine(path,
                $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss-FFFFFF")}-{filename}.txt");

            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public static void WriteGoalMapToLog(GoalMap goalMap, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Goal Map:");

            var stringBuilder = WriteGoalMap(goalMap);

            WriteToLog(logger, stringBuilder);
        }

        public static void WriteToLog(ILogger logger, StringBuilder stringBuilder)
        {
            foreach (var line in stringBuilder.ToString().Split(Environment.NewLine))
            {
                logger.Debug(line);
            }
        }

        public static StringBuilder WriteGoalMap(GoalMap goalMap)
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

        public static void WriteMapToFile(MarsMap map)
        {
            var stringBuilder = WriteMapUtc(map);
            
            WriteToTempFile("Map", stringBuilder);
        }

        public static void WriteMapToLog(MarsMap map, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug($"Map Level: {map.Level} | Id: {map.Id}");

            var stringBuilder = WriteMapUtc(map);
            
            WriteToLog(logger, stringBuilder);
        }

        public static StringBuilder WriteTexture(Texture2D texture, Dictionary<Color, char> colourToChar)
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
        
        public static void WriteTextureToFile(Texture2D texture, Dictionary<Color, char> colourToChar)
        {
            var stringBuilder = WriteTexture(texture, colourToChar);
            
            WriteToTempFile("Texture", stringBuilder);
        }
        
        public static void WriteTextureToLog(Texture2D texture, Dictionary<Color, char> colourToChar, ILogger logger)
        {
            if (!logger.IsEnabled(LogEventLevel.Debug))
                return;

            logger.Debug("Texture:");

            var stringBuilder = WriteTexture(texture, colourToChar);
            
            WriteToLog(logger, stringBuilder);
        }

        public static StringBuilder WriteMapUtc(MarsMap map)
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
                        .OfType<IMarsGameObject>()
                        .ToList();

                    var firstObject = gameObjects.First();

                    stringBuilder.Append(firstObject.AsciiCharacter);
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }
        
        public static StringBuilder WriteMapAscii(MarsMap map)
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
                        .OfType<IMarsGameObject>()
                        .ToList();

                    var firstObject = gameObjects.First();

                    var c = '?';

                    switch (firstObject)
                    {
                        case Item item:
                            switch (item.ItemType)
                            {
                                case NanoFlask _:
                                    c = '6';
                                    break;
                                case Gadget _:
                                    c = '2';
                                    break;
                                case Weapon _:
                                    c = '1';
                                    break;
                                case ShipRepairParts _:
                                    c = '7';
                                    break;
                            }
                            break;
                        case Wall _:
                            c = '#';
                            break;
                        case Floor _:
                            c = '.';
                            break;
                        default:
                            c = firstObject.AsciiCharacter;
                            break;
                    }

                    stringBuilder.Append(c);
                }
                
                
                if (y < map.Height - 1)
                    stringBuilder.AppendLine();
            }

            return stringBuilder;
        }
        
        public static string[] WriteMapAsciiArray(MarsMap map)
        {
            return WriteMapAscii(map).ToString().Split(Environment.NewLine);
        }
        
        public static void WriteMapAsciiToFile(MarsMap map)
        {
            WriteToTempFile("Map", WriteMapAscii(map));
        }
        
        public static string[] WriteGridView(IGridView<bool> arrayView, char trueChar = '.', char falseChar = '#')
        {
            return WriteGridView(arrayView, b => b ? trueChar : falseChar);
        }
        
        public static string[] WriteArrayView(ArrayView<bool> arrayView, char trueChar = '.', char falseChar = '#')
        {
            return WriteArrayView(arrayView, b => b ? trueChar : falseChar);
        }
        
        public static string[] WriteArrayView<T>(ArrayView<T> arrayView, Func<T, char> converter)
        {
            return WriteGridView(arrayView, converter);
        }
        
        public static string[] WriteGridView<T>(IGridView<T> gridView, Func<T, char> converter)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < gridView.Height; y++)
            {
                for (var x = 0; x < gridView.Width; x++)
                {
                    stringBuilder.Append(converter(gridView[x, y]));
                }

                if (y < gridView.Height - 1)
                    stringBuilder.AppendLine();
            }

            return stringBuilder.ToString().Split(Environment.NewLine);
        }
    }
}
