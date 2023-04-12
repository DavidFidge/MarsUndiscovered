using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.Graphics;

public class TileGraphicOptions
{
    public bool UseAsciiTiles { get; set; }
    public bool UseAnimations { get; set; }

    public TileGraphicOptions()
    {
    }

    public TileGraphicOptions(GameOptionsData gameOptionsData)
    {
        UseAsciiTiles = gameOptionsData.UseAsciiTiles;
        UseAnimations = gameOptionsData.UseAnimations;
    }
}