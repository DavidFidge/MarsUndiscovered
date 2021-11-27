using Augmented.Graphics.Models;

namespace Augmented.Interfaces
{
    public interface IAugmentedEntityFactory
    {
        AugmentedEntity Create();
        void Release(AugmentedEntity augmentedEntity);
    }
}