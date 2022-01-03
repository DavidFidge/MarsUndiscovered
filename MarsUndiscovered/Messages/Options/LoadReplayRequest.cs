using MediatR;

namespace MarsUndiscovered.Messages
{
    public class LoadReplayRequest : IRequest
    {
        public string Filename { get; set; }
    }
}