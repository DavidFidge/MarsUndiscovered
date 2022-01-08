using System;

using GeonBit.UI.Entities;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class BaseCompositeEntity
    {
        public abstract void AddAsChildTo(Entity parent);
        public abstract void RemoveFromParent();
    }
}