using System.Threading;
using System.Threading.Tasks;

using Augmented.Interfaces;
using Augmented.Messages;

using DavidFidge.MonoGame.Core.Graphics.Camera;

using MediatR;

namespace Augmented.Graphics.Camera
{
    public class GameView3D :
        IRequestHandler<Select3DViewRequest>,
        IRequestHandler<Action3DViewRequest>,
        IRequestHandler<Move3DViewRequest>,
        IRequestHandler<Rotate3DViewRequest>,
        IRequestHandler<Zoom3DViewRequest>
    {
        private readonly IGameCamera _camera;
        private readonly IAugmentedGameWorld _augmentedGameWorld;

        public GameView3D(IGameCamera gameCamera, IAugmentedGameWorld augmentedGameWorld)
        {
            _camera = gameCamera;
            _augmentedGameWorld = augmentedGameWorld;
        }

        public void StartNewGame()
        {
            _camera.Initialise();
        }

        public void Update()
        {
            _camera.Update();
        }

        public void Draw()
        {
            _augmentedGameWorld.SceneGraph.Draw(_camera.View, _camera.Projection);
        }

        public Task<Unit> Handle(Zoom3DViewRequest request, CancellationToken cancellationToken)
        {
            _camera.Zoom(request.Difference);
            return Unit.Task;
        }

        public Task<Unit> Handle(Move3DViewRequest request, CancellationToken cancellationToken)
        {
            _camera.GameUpdateContinuousMovement = request.CameraMovementFlags;

            return Unit.Task;
        }

        public Task<Unit> Handle(Rotate3DViewRequest request, CancellationToken cancellationToken)
        {
            if (request.XRotation > float.Epsilon)
                _camera.Rotate(CameraMovement.RotateDown, request.XRotation);
            else if (request.XRotation < float.Epsilon)
                _camera.Rotate(CameraMovement.RotateUp, -request.XRotation);

            if (request.ZRotation > float.Epsilon)
                _camera.Rotate(CameraMovement.RotateLeft, request.ZRotation);
            else if (request.ZRotation < float.Epsilon)
                _camera.Rotate(CameraMovement.RotateRight, -request.ZRotation);

            return Unit.Task;
        }

        public Task<Unit> Handle(Select3DViewRequest request, CancellationToken cancellationToken)
        {
            var ray = _camera.GetPointerRay(request.X, request.Y);

            _augmentedGameWorld.Select(ray);

            return Unit.Task;
        }

        public Task<Unit> Handle(Action3DViewRequest request, CancellationToken cancellationToken)
        {
            var ray = _camera.GetPointerRay(request.X, request.Y);

            _augmentedGameWorld.Action(ray);

            return Unit.Task;
        }
    }
}