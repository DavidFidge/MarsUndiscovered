using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components
{
    public class MapExit : Indestructible, IMementoState<MapExitSaveData>
    {
        public static char GetAsciiCharacter(MapExitDirection direction)
        {
            switch (direction)
            {
                case MapExitDirection.North:
                    return (char)0x1E;
                case MapExitDirection.South:
                    return (char)0x1F;
                case MapExitDirection.East:
                    return (char)0x10;
                case MapExitDirection.West:
                    return (char)0x11;
                case MapExitDirection.Up:
                    return (char)0x18;
                case MapExitDirection.Down:
                    return (char)0x19;
                default:
                    return ' ';
            }
        }

        public override char AsciiCharacter => GetAsciiCharacter(Direction);
        
        public string ExitText
        {
            get
            {
                switch (Direction)
                {
                    case MapExitDirection.North:
                        return "I walk north";
                    case MapExitDirection.South:
                        return "I walk south";
                    case MapExitDirection.East:
                        return "I walk east";
                    case MapExitDirection.West:
                        return "I walk west";
                    case MapExitDirection.Up:
                        return "I ascend";
                    case MapExitDirection.Down:
                        return "I descend";
                    default:
                        return string.Empty;
                }
            }
        }
                
        public string HoverText
        {
            get
            {
                switch (Direction)
                {
                    case MapExitDirection.North:
                        return "An exit north";
                    case MapExitDirection.South:
                        return "An exit south";
                    case MapExitDirection.East:
                        return "An exit east";
                    case MapExitDirection.West:
                        return "An exit west";
                    case MapExitDirection.Up:
                        return "A stairway upwards";
                    case MapExitDirection.Down:
                        return "A stairway downwards";
                    default:
                        return string.Empty;
                }
            }
        }

        public MapExit Destination { get; set; }
        public Point LandingPosition { get; set; }
        public MapExitDirection Direction { get; set; }

        public MapExit(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
        }

        public MapExit WithDestination(MapExit mapExit)
        {
            Destination = mapExit;
            return this;
        }

        public MapExit WithLandingPosition(Point landingPosition)
        {
            LandingPosition = landingPosition;
            return this;
        }

        public MapExit WithDirection(MapExitDirection direction)
        {
            Direction = direction;
            return this;
        }

        public IMemento<MapExitSaveData> GetSaveState()
        {
            var memento = new Memento<MapExitSaveData>(new MapExitSaveData());

            PopulateSaveState(memento.State);

            memento.State.DestinationId = Destination.ID;
            memento.State.LandingPosition = LandingPosition;
            memento.State.Direction = Direction;

            return memento;
        }

        public void SetLoadState(IMemento<MapExitSaveData> memento)
        {
            PopulateLoadState(memento.State);

            LandingPosition = memento.State.LandingPosition;
            Direction = memento.State.Direction;
            // Destination needs to be populated in MapExitCollection after all map exits are loaded
        }
    }
}
