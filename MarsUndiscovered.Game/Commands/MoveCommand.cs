using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class MoveCommand : BaseMarsGameActionCommand
    {
        public IGameObject GameObject { get; set; }
        public Tuple<Point, Point> FromTo { get; set; }

        public MoveCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(IGameObject gameObject, Tuple<Point, Point> fromTo)
        {
            GameObject = gameObject;
            FromTo = fromTo;
        }

        protected override CommandResult ExecuteInternal()
        {
            var subsequentCommands = new List<BaseGameActionCommand>();

            GameObject.Position = FromTo.Item2;

            if (GameObject is Player)
            {
                var item = GameObject.CurrentMap.GetObjectAt<Item>(GameObject.Position);

                if (item != null)
                {
                    var pickUpItemCommand = CommandCollection.CreateCommand<PickUpItemCommand>(GameWorld);
                    pickUpItemCommand.Initialise(item, GameObject);
                    subsequentCommands.Add(pickUpItemCommand);
                }
            }

            Mediator.Publish(new MapTileChangedNotification(FromTo.Item1));
            Mediator.Publish(new MapTileChangedNotification(FromTo.Item2));

            return Result(CommandResult.Success(this, subsequentCommands));
        }
    }
}
