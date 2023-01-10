using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.Messages.Console;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.ConsoleCommands;
using FrigidRogue.MonoGame.Core.Interfaces.ConsoleCommands;
using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class ConsoleViewModel : BaseViewModel<ConsoleData>,
        IRequestHandler<ExecuteConsoleCommandRequest>,
        IRequestHandler<RecallConsoleHistoryBackRequest>,
        IRequestHandler<RecallConsoleHistoryForwardRequest>
    {
        private readonly IConsoleCommandServiceFactory _consoleCommandServiceFactory;
        private readonly LinkedList<ConsoleCommand> _historyRecall = new LinkedList<ConsoleCommand>();
        private LinkedListNode<ConsoleCommand> _historyRecallItem;

        public ConsoleViewModel(IConsoleCommandServiceFactory consoleCommandServiceFactory)
        {
            _consoleCommandServiceFactory = consoleCommandServiceFactory;
        }

        public Task<Unit> Handle(ExecuteConsoleCommandRequest request, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(Data.Command))
            {
                var consoleCommand = new ConsoleCommand(Data.Command);

                var command = _consoleCommandServiceFactory
                    .CommandFor(consoleCommand);

                if (command != null)
                    command.Execute(consoleCommand);
                else
                    consoleCommand.Result = "Command not found";

                Data.LastCommands.AddFirst(consoleCommand);

                if (Data.LastCommands.Count > 50)
                    Data.LastCommands.RemoveLast();

                _historyRecall.AddFirst(consoleCommand);

                Data.Command = String.Empty;

                _historyRecallItem = null;

                Notify();
            }

            return Unit.Task;
        }

        public Task<Unit> Handle(RecallConsoleHistoryBackRequest request, CancellationToken cancellationToken)
        {
            if (_historyRecallItem == null)
            {
                if (_historyRecall.Any())
                {
                    _historyRecallItem = _historyRecall.First;
                    Data.Command = _historyRecallItem.Value.ToString();
                    Notify();
                }
            }
            else if (_historyRecallItem.Next != null)
            {
                _historyRecallItem = _historyRecallItem.Next;
                Data.Command = _historyRecallItem.Value.ToString();
                Notify();
            }

            return Unit.Task;
        }

        public Task<Unit> Handle(RecallConsoleHistoryForwardRequest request, CancellationToken cancellationToken)
        {
            if (_historyRecallItem != null)
            {
                if (_historyRecallItem.Previous != null)
                {
                    _historyRecallItem = _historyRecallItem.Previous;
                    Data.Command = _historyRecallItem.Value.ToString();
                }
                else
                {
                    Data.Command = String.Empty;
                    _historyRecallItem = null;
                }

                Notify();
            }

            return Unit.Task;
        }
    }
}