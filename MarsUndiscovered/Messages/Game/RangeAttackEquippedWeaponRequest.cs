using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Messages
{
    [ActionMap(Name = "Ranged Attack", DefaultKey = Keys.F)]
    public class RangeAttackEquippedWeaponRequest : IRequest
    {
    }
}