using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Domain.Base;
using Domain.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Commands
{
    public class DeleteCommand : IRequest<DeleteCommandResult>
    {
        public string CommandName { get; set; }
    }

    public class DeleteCommandResult
    {
        public bool CommandIsDeleted { get; set; }
    }

    internal class DeleteCommandHandler : BaseHandler<DeleteCommand, DeleteCommandResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public DeleteCommandHandler(TwitchBotDbContext dbContext, ILogger<DeleteCommand> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<DeleteCommandResult> HandleInternal(DeleteCommand request, CancellationToken cancellationToken)
        {
            Command command = await _dbContext.Commands.FirstOrDefaultAsync(c => c.CommandName == request.CommandName, cancellationToken);

            if (command == null)
            {
                return new()
                {
                    CommandIsDeleted = false
                };
            }


            _ = _dbContext.Commands.Remove(command);
            _ = _dbContext.SaveChanges();

            return new()
            {
                CommandIsDeleted = true
            };
        }
    }
}