using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Components.SaveData;

using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class Player : Actor, IMementoState<PlayerSaveData>, ISaveable
    {
        public override string Name => "You";
        public override string NameSpecificArticleLowerCase => Name.ToLower();
        public override string NameGenericArticleLowerCase => Name.ToLower();
        public override string NameSpecificArticleUpperCase => Name;
        public override string NameGenericArticleUpperCase => Name;
        public override string PossessiveName => $"{Name.ToLower()}r";
        public override string ToHaveConjugation => "have";
        public bool IsVictorious { get; set; }

        public override Attack BasicAttack { get; } = new Attack(new Range<int>(5, 10));

        public Player(uint id) : base(id)
        {
            MaxHealth = BaseHealth;
            Health = MaxHealth;
        }

        public IMemento<PlayerSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<PlayerSaveData>(mapper);
        }

        public void SetLoadState(IMemento<PlayerSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper(memento, mapper);
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            saveGameService.SaveToStore(GetSaveState(saveGameService.Mapper));
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();
            SetLoadState(playerSaveData, saveGameService.Mapper);
        }
    }
}