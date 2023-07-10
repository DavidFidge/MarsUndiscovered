using FrigidRogue.MonoGame.Core.Components.MapPointChoiceRules;

namespace MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;

public abstract class MarsMapPointChoiceRule : MapPointChoiceRule
{
    public abstract void AssignMap(MarsMap map);
}