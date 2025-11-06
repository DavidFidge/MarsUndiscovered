using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components;

public class StorySaveData : BaseSaveData
{
    public bool HasVisitedMiningFacilityOutside { get; set; }
    public bool IsLevel2StoryActive { get; set; } = true;
    public uint Level2MinerLeaderId { get; set; }
    public bool HasMetLeader { get; set; }
}