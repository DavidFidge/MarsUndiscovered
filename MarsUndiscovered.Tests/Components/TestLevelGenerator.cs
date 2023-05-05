using Castle.Windsor;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components
{
    public class TestLevelGenerator : ILevelGenerator
    {
        private GameWorld _gameWorld;
        private IMapGenerator _mapGenerator;
        private readonly ILevelGenerator _originalLevelGenerator;
        public IMapGenerator MapGenerator { get; set; }
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public IShipGenerator ShipGenerator { get; set; }
        public IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }

        public TestLevelGenerator(GameWorld gameWorld, WindsorContainer container, IMapGenerator mapGenerator)
        {
            _gameWorld = gameWorld;
            _originalLevelGenerator = _gameWorld.LevelGenerator;

            _mapGenerator = mapGenerator ?? new BlankMapGenerator(_gameWorld.GameObjectFactory);

            MonsterGenerator = _originalLevelGenerator.MonsterGenerator;
            ItemGenerator = _originalLevelGenerator.ItemGenerator;
            ShipGenerator = _originalLevelGenerator.ShipGenerator;
            MiningFacilityGenerator = _originalLevelGenerator.MiningFacilityGenerator;
            MapExitGenerator = _originalLevelGenerator.MapExitGenerator;
        }

        public void CreateLevels()
        {
            _mapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory);
            _gameWorld.AddMapToGame(_mapGenerator.Map);

            _gameWorld.Player = _gameWorld.GameObjectFactory
                .CreateGameObject<Player>()
                .PositionedAt(new Point(_mapGenerator.Map.Width / 2,
                    _mapGenerator.Map.Height - 2 -
                    (Constants.ShipOffset -
                     1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
                .AddToMap(_mapGenerator.Map);
        }

        public void Initialise(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
            _originalLevelGenerator.Initialise(gameWorld);
        }

        public ProgressiveWorldGenerationResult CreateProgressive(ulong seed, int step,
            WorldGenerationTypeParams worldGenerationTypeParams)
        {
            _originalLevelGenerator.MapGenerator = _mapGenerator;
            return _originalLevelGenerator.CreateProgressive(seed, step, worldGenerationTypeParams);
        }
    }
}