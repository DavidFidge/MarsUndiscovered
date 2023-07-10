using FrigidRogue.MonoGame.Core.Components.MapPointChoiceRules;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components
{
    public abstract class BaseSpawnGameObjectParams
    {
        public Guid MapId { get; set; }
        public Point Position { get; set; } = Point.None;

        public List<MapPointChoiceRule> MapPointChoiceRules { get; set; } = new List<MapPointChoiceRule>();
        
        public void AssignMap(MarsMap map)
        {
            foreach (var rule in MapPointChoiceRules)
            {
                if (rule is MarsMapPointChoiceRule marsMapPointChoiceRule)
                    marsMapPointChoiceRule.AssignMap(map);
            }
        }
    }

    public static class BaseSpawnGameObjectParamsFluentExtensions
    {
        public static T AtPosition<T>(this T spawnParams, Point point) where T : BaseSpawnGameObjectParams
        {
            spawnParams.Position = point;

            return spawnParams;
        }
        
        public static T OnMap<T>(this T spawnParams, Guid mapId) where T : BaseSpawnGameObjectParams
        {
            spawnParams.MapId = mapId;

            return spawnParams;
        }

        public static T AvoidingPosition<T>(this T spawnParams, Point point, uint avoidPositionRange) where T : BaseSpawnGameObjectParams
        {
            spawnParams.MapPointChoiceRules.Add(new MinDistanceRule(point, avoidPositionRange));

            return spawnParams;
        }
    }
}