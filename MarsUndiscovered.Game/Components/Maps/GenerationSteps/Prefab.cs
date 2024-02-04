using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class Prefab
{
    public string[] PrefabText { get; set; }
    public Rectangle Bounds => new Rectangle(0, 0, PrefabText[0].Length, PrefabText.Length);
}