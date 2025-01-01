using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyForcePushCommand : BaseMarsGameActionCommand<ApplyForcePushCommandSaveData>
    {
        public Item Source => GameWorld.Items[_data.SourceId];
        public Point Point => _data.Point;
        public int PushDistance => _data.PushDistance;
        public int Radius => _data.Radius;
        
        public ApplyForcePushCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item source, Point point, int pushDistance, int radius)
        {
           EndsPlayerTurn = false;
            _data.SourceId = source.ID;
            _data.Point = point;
            _data.PushDistance = pushDistance;
            _data.Radius = radius;
        }
        
        protected override CommandResult ExecuteInternal()
        {
            var currentMap = GameWorld.CurrentMap;

            var neighbours = Point.NeighboursOutwardsFrom(Radius, currentMap);

            var actors = new List<Actor>();
            
            foreach (var neighbour in neighbours)
            {
                var actor = currentMap.GetObjectAt<Actor>(neighbour);
                
                if (actor != null)
                    actors.Add(actor);
            }
            
            actors = actors
                .OrderByDescending(a => Point.EuclideanDistanceMagnitude(Point, a.Position))
                .ToList();

            foreach (var actor in actors)
            {
                var maxDistanceOut = Math.Max(
                    Math.Abs(Point.X - actor.Position.X),
                    Math.Abs(Point.Y - actor.Position.Y));
               
                var newPosition = Vector2.Lerp(Point.ToVector(), actor.Position.ToVector(), (PushDistance / (float)maxDistanceOut) + 1f);

                var newPoint = newPosition
                    .FromVectorRounded()
                    .Clamp(GameWorld.CurrentMap.Bounds());

                var line = Lines.GetBresenhamLine(actor.Position, newPoint);

                var selectedPoint = line.First();
                
                foreach (var linePoint in line.Skip(1))
                {
                    if (GameWorld.CurrentMap.AnyBlockingObjectsAt(linePoint))
                        break;

                    selectedPoint = linePoint;
                }

                if (selectedPoint != actor.Position)
                {
                    var moveCommand = GameWorld.CommandCollection.CreateCommand<MoveCommand>(GameWorld);
                    moveCommand.Initialise(actor, new Tuple<Point, Point>(actor.Position, selectedPoint));
                    
                    moveCommand.Execute();
                }
            }
            
            var message = "A force field radiates out from me!";
            var commandResult = CommandResult.Success(this, message);
            
            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
        }
    }
}
