using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class WorldBuilderViewModel : BaseGameCoreViewModel<WorldBuilderData>
    {
        public int CurrentStep { get; private set; }
        public bool IsFinalStep { get; private set; }
        public ulong Seed { get; private set; }
        public WorldGenerationTypeParams WorldGenerationTypeParams { get; private set; }
        
        public void BuildWorld(WorldGenerationTypeParams worldGenerationTypeParams)
        {
            IsActive = false;
            var result = GameWorldEndpoint.ProgressiveWorldGeneration(null, 1, worldGenerationTypeParams);
            IsActive = true;
            SetUpGameCoreViewModels();
            GameWorldEndpoint.AfterProgressiveWorldGeneration();
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
            if (IsFinalStep)
                return;

            CurrentStep++;
            
            var result = GameWorldEndpoint.ProgressiveWorldGeneration(Seed, CurrentStep, WorldGenerationTypeParams);
            GameWorldEndpoint.AfterProgressiveWorldGeneration();

            IsFinalStep = result.IsFinalStep;
            
            Mediator.Publish(new RefreshViewNotification());
        }

        public void PreviousStep()
        {
            if (CurrentStep == 1)
                return;

            CurrentStep--;
            var result = GameWorldEndpoint.ProgressiveWorldGeneration(Seed, CurrentStep, WorldGenerationTypeParams);
            GameWorldEndpoint.AfterProgressiveWorldGeneration();

            IsFinalStep = result.IsFinalStep;
            
            Mediator.Publish(new RefreshViewNotification());
        }
    }
}