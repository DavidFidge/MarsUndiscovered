using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components;

public class EnvironmentalEffectSaveData : GameObjectSaveData
{
    public string EnvironmentalEffectTypeName { get; set; }
    public bool IsRemoved { get; internal set; }
    public int Damage { get; internal set; }
    public int Duration { get; internal set; }
}