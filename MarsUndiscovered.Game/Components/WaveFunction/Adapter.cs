namespace MarsUndiscovered.Game.Components.WaveFunction;

public class Adapter
{
    public string Pattern { get; set; }

    public static implicit operator Adapter(string pattern)
    {
        return new Adapter { Pattern = pattern };
    }
}