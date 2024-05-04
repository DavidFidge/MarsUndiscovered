namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class TestPrefabProvider : IPrefabProvider
{
    public List<Prefab> Prefabs => _prefabs;
    private List<Prefab> _prefabs = new List<Prefab>();

    public TestPrefabProvider()
    {
        _prefabs.Add(new Prefab(new[]
        {
            "XCX",
            "C.C",
            "XCX"
        }));
    }
}