using AutoMapper;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor<MonsterSaveData>
    {
        private Breed Breed { get; set; }

        public int Health { get; set; }

        public Monster() : base(1, true)
        {
        }

        public Monster(uint id) : base(1, true, idGenerator: () => id)
        {
        }

        public override IMemento<MonsterSaveData> GetState(IMapper mapper)
        {
            return base.GetState(mapper);
        }
    }
}