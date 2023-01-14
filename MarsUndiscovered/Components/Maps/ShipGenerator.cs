using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components.Maps
{
    public class ShipGenerator : BaseGameObjectGenerator, IShipGenerator
    {
        /// <summary>
        /// Creates the player's ship. Currently uses hardcoded positions.
        /// </summary>
        /// <param name="gameObjectFactory"></param>
        /// <param name="shipCollection"></param>
        public void CreateShip(IGameObjectFactory gameObjectFactory, MarsMap map, ShipCollection shipCollection)
        {
            var lines = new[]
            {
                "XXXXXXXXXXXXXXXXX",
                "XXXXXXXXXX.-+--.X",
                "XX.------`  |--`X",
                "X{ (|       |XXXX",
                "XX`------.  |--.X",
                "XXXXXXXXXX`-+--`X",
                
            };
            // ship is in the middle of the x axis
            var shipStartX = (map.Width / 2) - lines[0].Length / 2;

            // ship is at the bottom of the screen.  Subtract by 2 to get it off the border 
            var shipStartY = map.Height - Constants.ShipOffset - lines.Length;

            for (var y = shipStartY; y < shipStartY + lines.Length; y++)
            {
                for (var x = shipStartX; x < shipStartX + lines[0].Length; x++)
                {
                    var shipPart = lines[y - shipStartY][x - shipStartX];

                    var point = new Point(x, y);

                    map.CreateFloor(point, gameObjectFactory);

                    if (map.GetObjectsAt(point).Any(o => !(o is Floor)))
                        throw new Exception("CreateShip must be done as the first object creation steps");

                    if (shipPart == 'X')
                        continue; // This is a blank square, so no ship part, but we still want to clear out any nearby walls otherwise the map may have unreachable spots

                    var ship = gameObjectFactory.CreateShip()
                        .PositionedAt(point)
                        .WithShipPart(lines[y - shipStartY][x - shipStartX]);

                    shipCollection.Add(ship.ID, ship);

                    map.AddEntity(ship);
                }
            }
        }
    }
}
