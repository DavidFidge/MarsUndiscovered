using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Tests.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class DeathCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void DeathCommand_On_Monster_Should_Set_IsDead_Flag_And_Remove_From_Map()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach"));

            var monster = _gameWorld.Monsters.Values.First();
            var oldMonsterPosition = monster.Position;
            var commandFactory = Container.Resolve<ICommandFactory>();

            var deathCommand = commandFactory.CreateDeathCommand(_gameWorld);
            deathCommand.Initialise(monster, "you");

            // Act
            var result = deathCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsFalse(_gameWorld.CurrentMap.GetObjectsAt(oldMonsterPosition).Any(m => m is Monster));
            Assert.IsTrue(monster.IsDead);
            Assert.AreEqual("The roach has died!", result.Messages[0]);
            Assert.AreEqual("killed by you", ((DeathCommand)result.Command).KilledByMessage);
        }

        [TestMethod]
        public void DeathCommand_On_Player_Should_Set_IsDead_Flag()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            var commandFactory = Container.Resolve<ICommandFactory>();

            var deathCommand = commandFactory.CreateDeathCommand(_gameWorld);
            deathCommand.Initialise(_gameWorld.Player, "a monster");

            // Act
            var result = deathCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);

            // Player is not removed
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt(_gameWorld.Player.Position).Any(m => m is Player));
            Assert.IsTrue(_gameWorld.Player.IsDead);
            Assert.AreEqual("You have died!", result.Messages[0]);
            Assert.AreEqual("killed by a monster", ((DeathCommand)result.Command).KilledByMessage);
        }
    }
}
