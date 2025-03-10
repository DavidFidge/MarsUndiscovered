using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Game.Components
{
    public class MapExit : Indestructible, IMementoState<MapExitSaveData>
    {
        public MapExitType MapExitType { get; set; }

        public override char AsciiCharacter => MapExitType.AsciiCharacter;
        public Color? BackgroundColour => MapExitType.BackgroundColour;
        public Color ForegroundColour => MapExitType.ForegroundColour;

        public string ExitText => MapExitType.ExitText;
        public string HoverText => MapExitType.HoverText;

        public MapExit Destination { get; set; }
        public Point LandingPosition { get; set; }

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

        public MapExit WithType(MapExitType mapExitType)
        {
            MapExitType = mapExitType;
            return this;
        }

        public IMemento<MapExitSaveData> GetSaveState()
        {
            var memento = new Memento<MapExitSaveData>(new MapExitSaveData());

            PopulateSaveState(memento.State);

            memento.State.DestinationId = Destination.ID;
            memento.State.LandingPosition = LandingPosition;
            memento.State.MapExitTypeName = MapExitType.Name;

            return memento;
        }

        public void SetLoadState(IMemento<MapExitSaveData> memento)
        {
            PopulateLoadState(memento.State);

            LandingPosition = memento.State.LandingPosition;
            MapExitType = MapExitType.MapExitTypes[memento.State.MapExitTypeName];

            // Destination is populated in MapExitCollection after all map exits are loaded
        }
    }
}
