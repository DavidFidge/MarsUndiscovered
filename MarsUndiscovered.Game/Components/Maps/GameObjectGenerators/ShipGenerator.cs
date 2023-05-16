using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;

namespace MarsUndiscovered.Game.Components.Maps
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
                "XXXXXXXXXX`-+--`X"
                
            };
            
            // ship is in the middle of the x axis
            var shipStartX = (map.Width / 2) - lines[0].Length / 2;

            // ship is at the bottom of the screen.  Subtract by 2 to get it off the border 
            var shipStartY = map.Height - Constants.ShipOffset - lines.Length;
            
            var mapTemplate = new MapTemplate(lines, shipStartX, shipStartY);

            foreach (var item in mapTemplate)
            {
                map.CreateFloor(FloorType.BlankFloor, item.Point, gameObjectFactory);

                if (map.GetObjectsAt(item.Point).Any(o => !(o is Floor)))
                    throw new Exception("CreateShip must be done as the first object creation steps");

                if (item.Char == 'X')
                    continue; // This is a blank square, so no ship part, but we still want to clear out any nearby walls otherwise the map may have unreachable spots

                var ship = gameObjectFactory.CreateGameObject<Ship>()
                    .PositionedAt(item.Point)
                    .WithShipPart(item.Char);

                shipCollection.Add(ship.ID, ship);

                map.AddEntity(ship);
            }
        }
    }
}
