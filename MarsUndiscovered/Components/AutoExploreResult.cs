using System.Collections.Generic;
using System.Linq;

using GoRogue.Pathing;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class AutoExploreResult
    {
        public Path Path { get; set; }
        public bool HasMonsterNearby { get; set; }

        public AutoExploreResult(GoalMap goalMap, GameWorld gameWorld)
        {
            var path = new HashSet<Point>();
            var startPosition = gameWorld.Player.Position;
            var endPosition = gameWorld.Player.Position;

            path.Add(startPosition);
            
            while (goalMap.GetDirectionOfMinValue(endPosition, false) != Direction.None && !path.Contains(endPosition + goalMap.GetDirectionOfMinValue(endPosition, false)))
            {
                endPosition += goalMap.GetDirectionOfMinValue(endPosition, false);
                path.Add(endPosition);
            }

            if (gameWorld.CurrentMap.GetObjectsAround<Monster>(gameWorld.Player.Position, AdjacencyRule.EightWay).Any())
                HasMonsterNearby = true;

            Path = new Path(path);
        }
    }
}