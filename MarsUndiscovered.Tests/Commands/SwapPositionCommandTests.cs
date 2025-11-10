using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class SwapPositionCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        [DataRow(ActorAllegianceState.Ally)]
        [DataRow(ActorAllegianceState.Friendly)]
        [DataRow(ActorAllegianceState.Neutral)]
        public void Swap_Positions_With_Ally(ActorAllegianceState state)
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));

            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.ActorAllegiances.Change(AllegianceCategory.Player, AllegianceCategory.Monsters, state);

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<SwapPositionCommand>().Count);
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);
            Assert.AreEqual(new Point(0, 0), monster.Position);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);
        }
    }
}
