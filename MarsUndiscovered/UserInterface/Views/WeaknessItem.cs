using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views;

public class WeaknessItem : BaseCompositeEntity
{
    private readonly Image _weaknessImage;
    private readonly Texture2D _activeTexture;
    private readonly Texture2D _inactiveTexture;
        
    public WeaknessItem(IAssets assets, string weaknessName)
    {
        _weaknessImage = new Image()
            .Anchor(Anchor.AutoInlineNoBreak)
            .Size(64, 64)
            .NoPadding();
        
        _activeTexture = assets.GetStaticTexture($"{weaknessName}Active");
        _inactiveTexture = assets.GetStaticTexture($"{weaknessName}Inactive");
            
        _weaknessImage.Texture = _inactiveTexture;
    }
        
    public void Activate()
    {
        _weaknessImage.Texture = _activeTexture;
    }
        
    public void Deactivate()
    {
        _weaknessImage.Texture = _inactiveTexture;
    }
        
    public void ActivateDeactivate(bool isActive)
    {
        _weaknessImage.Texture = isActive ? _activeTexture : _inactiveTexture;
    }

    public override void AddAsChildTo(Entity parent)
    {
        parent.AddChild(_weaknessImage);
    }

    public override void RemoveFromParent()
    {
        _weaknessImage.RemoveFromParent();
    }
}