using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components;

public class StorySaveData : BaseSaveData
{
    public bool HasVisitedMiningFacilityOutside { get; set; }
    public bool IsLevel2StoryActive { get; set; } = true;
    public uint Level2MinerLeaderId { get; set; }
    public bool HasMetMinerLeader { get; set; }

    public bool IsLevel3StoryActive { get; set; } = true;
    public bool HasGuidedMinerLeaderDown { get; set; }
    public bool HasGuidedMinerLeaderToCanteen { get; set; }

    public int LastLevel { get; set; }
    public int CurrentLevel { get; set; }

    public bool HasSwitchedLevels => LastLevel != CurrentLevel;
}