using MediatR;

namespace MarsUndiscovered.Messages
{
    public class LoadGameRequest : IRequest
    {
        public string Filename { get; set; }
    }
}