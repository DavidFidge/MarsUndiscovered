using System;

using Augmented.Interfaces;

using DavidFidge.MonoGame.Core.Components;
using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Graphics.Extensions;
using DavidFidge.MonoGame.Core.Graphics.Terrain;
using DavidFidge.MonoGame.Core.Interfaces.Components;
using DavidFidge.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace Augmented.Graphics.TerrainSpace
{
    public class Terrain : Entity, IDrawable, ILoadContent
    {
        private readonly IHeightMapGenerator _heightMapGenerator;
        private readonly IGameProvider _gameProvider;
        private readonly IAssetProvider _assetProvider;
        private IndexBuffer _terrainIndexBuffer;
        private VertexBuffer _terrainVertexBuffer;
        private Effect _effect;
        private SamplerState _samplerState;
        private HeightMap _heightMap;
        private Vector3 _scale;

        public Terrain(
            IHeightMapGenerator heightMapGenerator,
            IGameProvider gameProvider,
            IAssetProvider assetProvider)
        {
            _heightMapGenerator = heightMapGenerator;
            _gameProvider = gameProvider;
            _assetProvider = assetProvider;

            _samplerState = new SamplerState
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
            };
        }

        public VertexPositionNormalTexture[] CreateTerrainVertices()
        {
            var width = _heightMap.Width;
            var height = _heightMap.Length;

            var terrainVertices = new VertexPositionNormalTexture[width * height];

            var i = 0;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var position = new Vector3(x, y, _heightMap[x, y]);
                    var normal = new Vector3(0, 0, 1f);
                    var texture = new Vector2(x / (width / 10f), y / (height / 10f));

                    terrainVertices[i++] = new VertexPositionNormalTexture(position, normal, texture);
                }
            }

            return terrainVertices;
        }

        public int[] CreateTerrainIndexes()
        {
            var width = _heightMap.Width;
            var height = _heightMap.Length;
            var terrainIndexes = new int[width * 2 * (height - 1)];

            var i = 0;
            var y = 0;

            while (y < height - 1)
            {
                // create triangle strip indexes going forwards
                for (var x = 0; x < width; x++)
                {
                    terrainIndexes[i++] = x + y * width;
                    terrainIndexes[i++] = x + (y + 1) * width;
                }

                // move up to next row and create triangle strip indexes going backwards
                y++;

                if (y < height - 1)
                {
                    for (var x = width - 1; x >= 0; x--)
                    {
                        terrainIndexes[i++] = x + (y + 1) * width;
                        terrainIndexes[i++] = x + y * width;
                    }
                }

                y++;
            }

            return terrainIndexes;
        }

        public void CreateHeightMap(TerrainParameters terrainParameters)
        {
            _scale = new Vector3(20f, 20f, 0.005f) * GetScale(terrainParameters);

            LocalTransform.ChangeScale(_scale);

            var hillHeight = GetHillHeight(terrainParameters);

            var heightMapSize = 32;

            _heightMap = _heightMapGenerator
                .CreateHeightMap(heightMapSize + 1, heightMapSize + 1)
                .DiamondSquare(heightMapSize, -hillHeight, hillHeight, new SubtractingHeightsReducer())
                .HeightMap();

            var min = new Vector3(0, 0, _heightMap.Min);
            var max = new Vector3(_heightMap.Width - 1, _heightMap.Length - 1, _heightMap.Max);
        }

        private int GetHillHeight(TerrainParameters terrainParameters)
        {
            var hillHeight = 20000;

            switch (terrainParameters.HillHeight)
            {
                case TerrainSpace.HillHeight.Low:
                    hillHeight /= 2;
                    break;
                case TerrainSpace.HillHeight.High:
                    hillHeight *= 2;
                    break;
            }

            return hillHeight;
        }

        private Vector3 GetScale(TerrainParameters terrainParameters)
        {
            var scale = Vector3.One;

            switch (terrainParameters.Size)
            {
                case WorldSize.Small:
                    scale.X *= 0.5f;
                    scale.Y *= 0.5f;
                    break;
                case WorldSize.Big:
                    scale.X *= 2f;
                    scale.Y *= 2f;
                    break;
            }

            return scale;
        }

        public void LoadContent()
        {
            if (_heightMap == null)
                throw new Exception("Create height map first");

            var terrainVertices = CreateTerrainVertices();
            var terrainIndexes = CreateTerrainIndexes();

            terrainVertices.GenerateNormalsForTriangleStrip(terrainIndexes);

            if (_terrainVertexBuffer == null || _terrainVertexBuffer.VertexCount != terrainVertices.Length)
            {
                _terrainVertexBuffer = new VertexBuffer(
                    _gameProvider.Game.GraphicsDevice,
                    VertexPositionNormalTexture.VertexDeclaration,
                    terrainVertices.Length,
                    BufferUsage.WriteOnly
                );
            }

            _terrainVertexBuffer.SetData(terrainVertices);

            if (_terrainIndexBuffer == null || _terrainIndexBuffer.IndexCount != terrainIndexes.Length)
            {
                _terrainIndexBuffer = new IndexBuffer(
                    _gameProvider.Game.GraphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    terrainIndexes.Length,
                    BufferUsage.WriteOnly
                );
            }

            _terrainIndexBuffer.SetData(terrainIndexes);

            if (_effect == null)
            {
                _effect = _gameProvider.Game.EffectCollection.BuildTextureEffect(_assetProvider.GrassTexture);
            }
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            var graphicsDevice = _gameProvider.Game.GraphicsDevice;

            graphicsDevice.Indices = _terrainIndexBuffer;
            graphicsDevice.SetVertexBuffer(_terrainVertexBuffer);
            var oldSamplerState = graphicsDevice.SamplerStates[0];
            graphicsDevice.SamplerStates[0] = _samplerState;

            if (_effect != null)
            {
                _effect.SetWorldViewProjection(
                    world,
                    view,
                    projection
                );

                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleStrip,
                        0,
                        0,
                        _terrainIndexBuffer.IndexCount - 2
                    );
                }
            }

            graphicsDevice.SamplerStates[0] = oldSamplerState;
        }
        
        public Vector3? RayToTerrainPoint(Ray ray, ISceneGraph sceneGraph)
        {
            var worldTransform = sceneGraph.GetWorldTransformWithLocalTransform(this);
            var worldTransformInverse = Matrix.Invert(worldTransform);

            var translatedRay = new Ray(Vector3.Transform(ray.Position, worldTransformInverse), Vector3.Transform(Vector3.Normalize(ray.Direction), worldTransformInverse));

            var rayOverFirstHill = _heightMap.LinearSearch(translatedRay);

            if (rayOverFirstHill == null)
                return null;

            var approximatePositionOnHill = _heightMap.BinarySearch(rayOverFirstHill.Value);

            return Vector3.Transform(approximatePositionOnHill, worldTransform);
        }
    }
}