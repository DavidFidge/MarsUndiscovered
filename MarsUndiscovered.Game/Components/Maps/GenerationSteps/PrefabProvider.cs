namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class PrefabProvider : IPrefabProvider
{
    public List<Prefab> Prefabs => _prefabs;
    private List<Prefab> _prefabs = new List<Prefab>();

    public PrefabProvider()
    {
        _prefabs.Add(new Prefab(new[]
        {
            "XCCCCCCCX",
            "C.......C",
            "C.......C",
            "C.......C",
            "C.......C",
            "C.......C",
            "C.......C",
            "C.......C",
            "XCCCCCCCX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XCCX",
            "C..C",
            "C..C",
            "C..C",
            "C..C",
            "C..C",
            "C..C",
            "C..C",
            "XCCX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XCCX",
            "C..C",
            "C..C",
            "XCCX"
        }));
    }
}