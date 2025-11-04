using System.Diagnostics;
using System.Text;
using BehaviourTree;
using BehaviourTree.FluentBuilder;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.FOV;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.Random;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components
{
    public class EnvironmentalEffect : MarsGameObject, IMementoState<EnvironmentalEffectSaveData>
    {
        public EnvironmentalEffectType EnvironmentalEffectType { get; set; }
        
        public int Damage { get; set; }
        public int Duration { get; set; }
        public bool IsRemoved { get; set; }
        
        private IList<BaseGameActionCommand> _nextCommands;
        
        public EnvironmentalEffect(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.EnvironmentalEffectLayer, idGenerator: () => id)
        {
            _nextCommands = new List<BaseGameActionCommand>();
        }
        
        public IMemento<EnvironmentalEffectSaveData> GetSaveState()
        {
            var memento = new Memento<EnvironmentalEffectSaveData>(new EnvironmentalEffectSaveData());

            PopulateSaveState(memento.State);
            memento.State.EnvironmentalEffectTypeName = EnvironmentalEffectType.Name;
            memento.State.Damage = Damage;
            memento.State.Duration = Duration;
            memento.State.IsRemoved = IsRemoved;

            return memento;
        }
        
        public void SetLoadState(IMemento<EnvironmentalEffectSaveData> memento)
        {
            PopulateLoadState(memento.State);
            EnvironmentalEffectType = EnvironmentalEffectType.EnvironmentalEffectTypes[memento.State.EnvironmentalEffectTypeName];
            Damage = memento.State.Damage;
            Duration = memento.State.Duration;
            IsRemoved = memento.State.IsRemoved;
        }

        public EnvironmentalEffect WithEnvironmentalEffectType(EnvironmentalEffectType environmentalEffectType)
        {
            EnvironmentalEffectType = environmentalEffectType;

            Damage = environmentalEffectType.Damage;
            Duration = environmentalEffectType.Duration;
        
            return this;
        }

        public string GetAmbientText()
        {
            return EnvironmentalEffectType.GetAmbientText();
        }
        
        
        public IEnumerable<BaseGameActionCommand> NextTurn(ICommandCollection commandFactory)
        {
            _nextCommands.Clear();

            Duration--;
            
            if (Duration == 0)
            {
                var explodeTileCommand = commandFactory.CreateCommand<ExplodeTileCommand>(GameWorld);
                explodeTileCommand.Initialise(this.Position, this.Damage, this);

                _nextCommands.Add(explodeTileCommand);

                IsRemoved = true;
            }
            
            return _nextCommands;
        }
    }
}