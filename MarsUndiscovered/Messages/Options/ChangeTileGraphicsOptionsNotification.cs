using MarsUndiscovered.Graphics;
using MediatR;

namespace MarsUndiscovered.Messages
{
    public class ChangeTileGraphicsOptionsNotification : INotification
    {
        public TileGraphicOptions TileGraphicOptions { get; }

        public ChangeTileGraphicsOptionsNotification(TileGraphicOptions tileGraphicOptions)
        {
            TileGraphicOptions = tileGraphicOptions;
        }
    }
}