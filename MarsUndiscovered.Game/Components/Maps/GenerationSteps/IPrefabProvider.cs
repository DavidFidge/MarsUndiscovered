namespace MarsUndiscovered.Game.Components.GenerationSteps;

public interface IPrefabProvider
{
    // C = potential connection point
    // # = wall - can be tunneled into (unused space)
    // X = wall - cannot be tunneled into
    // . = floor
    List<Prefab> Prefabs { get; }
}