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
    public class AddCommand : IRequest<AddCommandResult>
    {
        public Command Command { get; set; }
    }

    public class AddCommandResult
    {
        public bool CommandIsAdded { get; set; }
    }

    internal class AddCommandHandler : BaseHandler<AddCommand, AddCommandResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public AddCommandHandler(TwitchBotDbContext dbContext, ILogger<AddCommand> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<AddCommandResult> HandleInternal(AddCommand request, CancellationToken cancellationToken)
        {
            if (await _dbContext.Commands.AnyAsync(c => c.CommandName == request.Command.CommandName, cancellationToken))
            {
                return new()
                {
                    CommandIsAdded = false
                };
            }

            _ = await _dbContext.AddAsync(request.Command, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return new()
            {
                CommandIsAdded = true
            };
        }
    }
}