using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

using MonoGame.Extended;

namespace MarsUndiscovered.Game.Components
{
    public class Player : Actor, IMementoState<PlayerSaveData>, ISaveable
    {
        public override char AsciiCharacter => '@';

        public const int BaseHealth = 100;
        public override string Name => "You";
        public override string NameSpecificArticleLowerCase => Name.ToLower();
        public override string NameGenericArticleLowerCase => Name.ToLower();
        public override string NameSpecificArticleUpperCase => Name;
        public override string NameGenericArticleUpperCase => Name;
        public override string PossessiveName => $"{Name.ToLower()}r";
        public override string ToHaveConjugation => "have";
        public bool IsVictorious { get; set; }
        public bool IsGameEndState => IsVictorious || IsDead;

        public Attack UnarmedAttack { get; set; } = new Attack(new Range<int>(2, 5));

        public override bool IsWallTurret { get; } = false;
        public int SenseRange { get; set; }

        public Player(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
            MaxHealth = 100;
            Health = MaxHealth;
            RegenRate = 0.005m;
            SenseRange = 5;
            InitialiseAttacks();
        }

        public IMemento<PlayerSaveData> GetSaveState()
        {
            var memento = new Memento<PlayerSaveData>(new PlayerSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.IsVictorious = IsVictorious;

            return memento;
        }

        public void SetLoadState(IMemento<PlayerSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            IsVictorious = memento.State.IsVictorious;
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            saveGameService.SaveToStore(GetSaveState());
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();
            SetLoadState(playerSaveData);
        }

        public void RecalculateAttacksForItem(Item item)
        {
            InitialiseAttacks();
            
            if (item != null)
            {
                MeleeAttack = (Attack)item.MeleeAttack?.Clone();
                LineAttack = (Attack)item.LineAttack?.Clone();
                LaserAttack = (LaserAttack)item.LaserAttack?.Clone();
                CanConcuss = item.CanConcuss;
            }
        }

        private void InitialiseAttacks()
        {
            MeleeAttack = (Attack)UnarmedAttack.Clone();
            LineAttack = null;
            LightningAttack = null;
            LaserAttack = null;
            CanConcuss = false;
        }
    }
}