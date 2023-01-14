using GoRogue.Random;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components.Maps
{
    public class MiningFacilityGenerator : BaseGameObjectGenerator, IMiningFacilityGenerator
    {
        /// <summary>
        /// Creates the mining facility that the player must enter on the first level
        /// </summary>
        /// <param name="gameObjectFactory"></param>
        /// <param name="miningFacilityCollection"></param>
        public void CreateMiningFacility(IGameObjectFactory gameObjectFactory, MarsMap map, MiningFacilityCollection miningFacilityCollection)
        {
            var lines = new[]
            {
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXX___XXXXXXX___XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXX) (XXXXXXX) (XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXX___X| |XX___XX| |XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXX) (X| |XX) (XX| |XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXX| |X| |XX| |XX| |XXXXXXXX/'-._XXXXXXXX/'-._XXXXXXXXXX",
                "XXXXXX.___| |_| |__| |__| |_______/     `'-____/     `'-.___.XX",
                "XXXX /    | | | |  | |  | |      /            /            /|XX",
                "XXX /     | |      | |          /            /            / |XX",
                "XX /      | |      | |         |'-._        |'-._        /  |XX",
                "X /       | |      | |         |    `'-._   |    `'-._  /  .|XX",
                "X------------------------------+          `-+          `' /:|XX",
                "X|        .----------.              #-#          #-#    |/ :/XX",
                "X|       #   .  .  .  #             #-#          #-#    || /==X",
                "X|       | .  .  .  . |                                 ||/===X",
                "X|_______+____________+_________________________________|/XXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
            };
            
            var miningFacilityStartX = GlobalRandom.DefaultRNG.NextInt(0, map.Width - lines[0].Length - 1);
            var miningFacilityStartY = 0;

            for (var y = miningFacilityStartY; y < miningFacilityStartY + lines.Length; y++)
            {
                for (var x = miningFacilityStartX; x < miningFacilityStartX + lines[0].Length; x++)
                {
                    var miningFacilitySection = lines[y - miningFacilityStartY][x - miningFacilityStartX];

                    var point = new Point(x, y);

                    map.CreateFloor(point, gameObjectFactory);

                    if (map.GetObjectsAt(point).Any(o => !(o is Floor)))
                        throw new Exception("CreateMiningFacility must be done as the first object creation steps");

                    if (miningFacilitySection == 'X')
                        continue; // This is a blank square, so no miningFacility part, but we still want to clear out any nearby walls otherwise the map may have unreachable spots

                    var miningFacility = gameObjectFactory.CreateMiningFacility()
                        .PositionedAt(point)
                        .WithMiningFacilitySection(lines[y - miningFacilityStartY][x - miningFacilityStartX]);

                    miningFacilityCollection.Add(miningFacility.ID, miningFacility);

                    map.AddEntity(miningFacility);
                }
            }
        }
    }
}
