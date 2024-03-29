using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components
{
    public class MapExit : Indestructible, IMementoState<MapExitSaveData>
    {
        public override char AsciiCharacter => Direction == Direction.Up ? '<' : '>';
        public MapExit Destination { get; set; }
        public Point LandingPosition { get; set; }
        public Direction Direction { get; set; }

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

        public MapExit WithDirection(Direction direction)
        {
            Direction = direction;
            return this;
        }

        public IMemento<MapExitSaveData> GetSaveState()
        {
            var memento = new Memento<MapExitSaveData>(new MapExitSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.DestinationId = Destination.ID;
            memento.State.LandingPosition = LandingPosition;
            memento.State.Direction = Direction;

            return memento;
        }

        public void SetLoadState(IMemento<MapExitSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            LandingPosition = memento.State.LandingPosition;
            Direction = memento.State.Direction;
            // Destination needs to be populated in MapExitCollection after all map exits are loaded
        }
    }
}
