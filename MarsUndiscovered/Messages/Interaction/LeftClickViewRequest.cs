using MediatR;

namespace MarsUndiscovered.Messages
{
    public class LeftClickViewRequest : BaseClickViewRequest
    {
        public LeftClickViewRequest(int x, int y) : base(x, y)
        {
        }
    }
}