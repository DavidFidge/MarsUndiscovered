using System;
using System.Collections.Generic;
using System.Text;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor, IMementoState<MonsterSaveData>
    {
        public Breed Breed { get; set; }
        public override string Name => Breed.Name;
        public override string Description => Breed.Description;
        public override Attack BasicAttack => Breed.BasicAttack;
        public override LightningAttack LightningAttack => Breed.LightningAttack;
        public override bool IsWallTurret => Breed.IsWallTurret;

        public MonsterGoal MonsterGoal { get; set; }

        public string GetInformation(Player player)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Name);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Description);
            stringBuilder.AppendLine();

            var percentMaxDamage = Breed.BasicAttack.DamageRange.Max * 100 / player.MaxHealth;
            var percentMinDamage = Breed.BasicAttack.DamageRange.Min * 100 / player.MaxHealth;
            var defeatTurns = player.Health / Breed.BasicAttack.DamageRange.Max;

            var percentText = percentMinDamage != percentMaxDamage
                ? $"between {percentMinDamage}-{percentMaxDamage}%"
                : $"{percentMinDamage}%";

            stringBuilder.AppendLine(
                $"{NameSpecificArticleUpperCase} can hit you for {percentText} of your maximum health and, at worst, could defeat you in {defeatTurns} hits."
            );

            return stringBuilder.ToString();
        }

        public Monster(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
            MonsterGoal = new MonsterGoal(this);
        }

        public Monster WithBreed(Breed breed)
        {
            Breed = breed;
            MaxHealth = (int)(BaseHealth * Breed.HealthModifier);
            Health = MaxHealth;

            return this;
        }

        public Monster AddToMap(MarsMap marsMap)
        {
            // Normally actors are not walkable as they can't be on the same square, but if an actor is on a wall it has to be walkable so that
            // it can be on the same square as a (non-walkable) wall.
            if (IsWallTurret)
                IsWalkable = true; 

            MarsGameObjectFluentExtensions.AddToMap(this, marsMap);

            MonsterGoal.ChangeMap();

            return this;
        }

        public void SetLoadState(IMemento<MonsterSaveData> memento)
        {
            PopulateLoadState(memento.State);
            Breed = Breed.Breeds[memento.State.BreedName];

            MonsterGoal = new MonsterGoal(this);
            MonsterGoal.SetLoadState(memento.State.MonsterGoalSaveData);
        }

        public IMemento<MonsterSaveData> GetSaveState()
        {
            var memento = new Memento<MonsterSaveData>(new MonsterSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.BreedName = Breed.Name;
            memento.State.MonsterGoalSaveData = MonsterGoal.GetSaveState();

            return memento;
        }

        public override void AfterMapLoaded()
        {
            base.AfterMapLoaded();

            MonsterGoal.AfterMapLoaded();
        }

        public IEnumerable<BaseGameActionCommand> NextTurn(ICommandFactory commandFactory)
        {
            var direction = MonsterGoal.GetNextMove(GameWorld);

            if (direction != Direction.None)
            {
                var positionBefore = Position;

                var positionAfter = Position.Add(direction);

                var player = CurrentMap.GetObjectAt<Player>(positionAfter);

                if (player != null)
                {
                    var attackCommand = commandFactory.CreateAttackCommand(GameWorld);
                    attackCommand.Initialise(this, player);

                    yield return attackCommand;
                }
                else
                {
                    var moveCommand = commandFactory.CreateMoveCommand(GameWorld);
                    moveCommand.Initialise(this, new Tuple<Point, Point>(positionBefore, positionAfter));

                    yield return moveCommand;
                }
            }
        }
    }
}