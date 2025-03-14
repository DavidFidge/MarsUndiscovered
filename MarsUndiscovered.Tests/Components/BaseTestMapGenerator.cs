﻿using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;

namespace MarsUndiscovered.Tests.Components
{
    public abstract class BaseTestMapGenerator : BaseMapGenerator
    {
        protected readonly IGameObjectFactory _gameObjectFactory;

        protected BaseTestMapGenerator(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }
    }
}