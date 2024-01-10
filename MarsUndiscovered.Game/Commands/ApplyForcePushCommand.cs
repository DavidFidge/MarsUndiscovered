using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

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

        public void Initialise(Item source, Point point, int pushDistance)
        {
            _data.SourceId = source.ID;
            _data.Point = point;
            _data.PushDistance = pushDistance;
            _data.Radius = 1;
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
                var slope = Point.BearingOfLine(Point, actor.Position);
                
                
                
                actor.Position.Translate(
                
                var path = new Path()
            }


            var message = $"A force field radiates out from you!";
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
            Target.Shield = _data.OldShieldAmount;
        }
    }
}
