﻿using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components;

public abstract class GameObjectType
{
    public char AsciiCharacter { get; set; }
    public Color ForegroundColour { get; set; }
    public Color? BackgroundColour { get; set; }
    public string Name { get; set; }

    // Second frame for animations with ascii
    public char AsciiCharacter2 { get; set; }
    public Color ForegroundColour2 { get; set; }
    public Color? BackgroundColour2 { get; set; }

    public GameObjectType()
    {
        Name = GetType().Name;
    }
    
    public virtual string GetTypeDescription()
    {
        return GetType().Name.ToSeparateWords();
    }
    
    public virtual string GetAbstractTypeName()
    {
        return nameof(GameObjectType);
    }
}