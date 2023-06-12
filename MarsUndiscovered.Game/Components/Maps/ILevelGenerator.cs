namespace MarsUndiscovered.Game.Components.Maps;

public interface ILevelGenerator
{
    void CreateLevels();
    void Initialise(GameWorld gameWorld);

    ProgressiveWorldGenerationResult CreateProgressive(ulong seed, int step, WorldGenerationTypeParams worldGenerationTypeParams);
    IMapGenerator MapGenerator { get; set; }
    IMonsterGenerator MonsterGenerator { get; set; }
    IItemGenerator ItemGenerator { get; set; }
    IShipGenerator ShipGenerator { get; set; }
    IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
    IMapExitGenerator MapExitGenerator { get; set; }
}