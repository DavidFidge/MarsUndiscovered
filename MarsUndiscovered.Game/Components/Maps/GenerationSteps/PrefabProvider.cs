namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class PrefabProvider : IPrefabProvider
{
    public List<Prefab> Prefabs => _prefabs;
    private List<Prefab> _prefabs = new List<Prefab>();

    public PrefabProvider()
    {
        // _prefabs.Add(new Prefab(new[]
        // {
        //     "XCCCCCCCX",
        //     "C.......C",
        //     "C.......C",
        //     "C.......C",
        //     "C.......C",
        //     "C.......C",
        //     "C.......C",
        //     "C.......C",
        //     "XCCCCCCCX"
        // }));

        _prefabs.Add(new Prefab(new[]
        {
            "XCCX",
            "C..C",
            "C..C",
            "XCCX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XCCCCCCX",
            "C......C",
            "C......C",
            "C......C",
            "C......C",
            "XCCCCCCX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XXXXCXXXX",
            "X..D.D..X",
            "XXXX.XXXX",
            "X..D.D..X",
            "XXXX.XXXX",
            "X..D.D..X",
            "XXXXCXXXX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XCCCCCCCCCCX",
            "C..........C",
            "C..XX..XX..C",
            "C..XX..XX..C",
            "C..........C",
            "XCCCCCCCCCCX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XCCCCCX",
            "C.....C",
            "C.X.X.C",
            "C.....C",
            "C.X.X.C",
            "C.....C",
            "XCCCCCX"
        }));
        
        _prefabs.Add(new Prefab(new[]
        {
            "XXXXXCXXXXX",
            "XXXX...XXXX",
            "XXX.....XXX",
            "XX.......XX",
            "C.........C",
            "XX.......XX",
            "XXX.....XXX",
            "XXXX...XXXX",
            "XXXXXCXXXXX"
        }));
    }
}