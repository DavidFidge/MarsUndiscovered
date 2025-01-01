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
        public override string Name => "I";
        public override string ObjectiveName => "Me";
        public override string PossessiveName => "My";
        public override string ToHaveConjugation => "have";
        public bool IsVictorious { get; set; }
        public bool IsGameEndState => IsVictorious || IsDead;

        public Attack UnarmedAttack { get; set; } = new Attack(new Range<int>(2, 5));

        public override bool IsWallTurret { get; } = false;

        public Player(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
            NameIsProperNoun = true;
            MaxHealth = 100;
            Health = MaxHealth;
            RegenRate = 0.005m;
            InitialiseAttacks();
        }

        public IMemento<PlayerSaveData> GetSaveState()
        {
            var memento = new Memento<PlayerSaveData>(new PlayerSaveData());

            PopulateSaveState(memento.State);

            memento.State.IsVictorious = IsVictorious;

            return memento;
        }

        public void SetLoadState(IMemento<PlayerSaveData> memento)
        {
            PopulateLoadState(memento.State);

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