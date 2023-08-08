using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class Wall : Terrain, IMementoState<WallSaveData>
    {
        private WallType _wallType;
        public WallType WallType
        {
            get
            {
                return _wallType;
            }
            set
            {
                _wallType = value;
                IsTransparent = _wallType.IsTransparent;
            }
        }
        
        public Wall(IGameWorld gameWorld, uint id) : base(gameWorld, id, false, false)
        {
        }

        public IMemento<WallSaveData> GetSaveState()
        {
            var memento = new Memento<WallSaveData>(new WallSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.WallTypeName = WallType.Name;

            return memento;
        }

        public void SetLoadState(IMemento<WallSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            WallType = WallType.WallTypes[memento.State.WallTypeName];
        }
    }
}