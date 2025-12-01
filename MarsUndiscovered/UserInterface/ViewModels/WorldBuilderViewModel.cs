using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class WorldBuilderViewModel : BaseGameCoreViewModel<WorldBuilderData>
    {
        public int CurrentStep { get; private set; }
        public bool IsFinalStep { get; private set; }
        public bool Failed { get; private set; }
        public ulong Seed { get; private set; }
        public WorldGenerationTypeParams WorldGenerationTypeParams { get; private set; }
        
        public void BuildWorld(WorldGenerationTypeParams worldGenerationTypeParams)
        {
            Failed = false;
            IsActive = false;
            var result = GameWorldProvider.ProgressiveWorldGeneration(null, 1, worldGenerationTypeParams);
            IsActive = true;
            SetUpGameCoreViewModels();
            GameWorldProvider.GameWorld.AfterProgressiveWorldGeneration();
            CurrentStep = 1;
            IsFinalStep = result.IsFinalStep;
            Seed = result.Seed;
            WorldGenerationTypeParams = worldGenerationTypeParams;

            Mediator.Publish(new RefreshViewNotification());
        }

        protected override void RefreshView()
        {
        }

        public void NextStep()
        {
            if (IsFinalStep || Failed)
                return;

            CurrentStep++;
            
            var result = GameWorldProvider.GameWorld.ProgressiveWorldGeneration(Seed, CurrentStep, WorldGenerationTypeParams);

            if (result.Failed)
            {
                Failed = true;
                return;
            }

            GameWorldProvider.GameWorld.AfterProgressiveWorldGeneration();

            IsFinalStep = result.IsFinalStep;
            
            Mediator.Publish(new RefreshViewNotification());
        }

        public void PreviousStep()
        {
            Failed = false;

            if (CurrentStep == 1)
                return;

            CurrentStep--;
            var result = GameWorldProvider.GameWorld.ProgressiveWorldGeneration(Seed, CurrentStep, WorldGenerationTypeParams);
            GameWorldProvider.GameWorld.AfterProgressiveWorldGeneration();

            IsFinalStep = result.IsFinalStep;
            
            Mediator.Publish(new RefreshViewNotification());
        }
    }
}