using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Components.SaveData;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class MapExit : MarsGameObject, IMementoState<MapExitSaveData>
    {
        public MapExit Destination { get; set; }
        public Point LandingPosition { get; set; }
        public Direction Direction { get; set; }

        public MapExit(uint id) : base(Constants.MapExitLayer, false, true, () => id)
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

        public IMemento<MapExitSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<MapExitSaveData>(mapper);
        }

        public void SetLoadState(IMemento<MapExitSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper<MapExitSaveData>(memento, mapper);
        }
    }
}