using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Interfaces;

using Newtonsoft.Json;

namespace MarsUndiscovered.Commands
{
    public class AttackCommand : BaseGameActionCommand<AttackCommandSaveData>
    {
        private IGameObject _source;
        private IGameObject _target;

        [JsonIgnore]
        public IGameWorld GameWorld { get; set; }

        public void Initialise(IGameObject source, IGameObject target)
        {
            _source = source;
            _target = target;
        }

        public override IMemento<AttackCommandSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<AttackCommandSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public override void SetLoadState(IMemento<AttackCommandSaveData> memento, IMapper mapper)
        {
            base.SetLoadState(memento, mapper);

            Memento<AttackCommandSaveData>.SetWithAutoMapper(this, memento, mapper);

            _source = GameWorld.GameObjects[memento.State.SourceId];
            _target = GameWorld.GameObjects[memento.State.TargetId];
        }

        protected override CommandResult ExecuteInternal()
        {
            throw new NotImplementedException();
        }

        protected override void UndoInternal()
        {
            throw new NotImplementedException();
        }
    }
}
