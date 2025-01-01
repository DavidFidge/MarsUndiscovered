using FrigidRogue.MonoGame.Core.Components;
using GoRogue.Pathing;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components
{
    public class AutoExploreResult
    {
        public Path Path { get; set; }
        public bool MovementInterrupted { get; set; }

        public IList<CommandResult> CommandResults { get; set; }

        public AutoExploreResult(
            IGridView<double?> goalMap,
            Player player,
            IList<CommandResult> moveRequestResults,
            IList<Monster> lastMonstersInView,
            IList<Monster> monstersInView)
        {
            if (moveRequestResults == null)
            {
                Path = new Path(new List<Point>());
                CommandResults = new List<CommandResult>();
                return;
            }
            
            CommandResults = moveRequestResults.ToList();

            var path = new HashSet<Point>();
            var startPosition = player.Position;
            var endPosition = player.Position;

            path.Add(startPosition);

            while (goalMap.GetDirectionOfMinValue(endPosition, AdjacencyRule.EightWay, false) != Direction.None && !path.Contains(endPosition + goalMap.GetDirectionOfMinValue(endPosition, AdjacencyRule.EightWay, false)))
            {
                endPosition += goalMap.GetDirectionOfMinValue(endPosition, AdjacencyRule.EightWay, false);
                path.Add(endPosition);
            }

            Path = new Path(path);

            if (player.CurrentMap.GetObjectsAround<Monster>(player.Position, AdjacencyRule.EightWay).Any()
                || (moveRequestResults != null && moveRequestResults.Any(r => r.Command.InterruptsMovement))
                || monstersInView.Except(lastMonstersInView).Any())
            {
                MovementInterrupted = true;
            }
        }
    }
}
