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
    public class EditCommand : IRequest<EditCommandResult>
    {
        public string CommandName { get; set; }
        public string CommandOutput { get; set; }
    }

    public class EditCommandResult
    {
        public bool CommandWasEdited { get; set; }
    }

    internal class EditCommandHandler : BaseHandler<EditCommand, EditCommandResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public EditCommandHandler(TwitchBotDbContext dbContext, ILogger<EditCommand> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<EditCommandResult> HandleInternal(EditCommand request, CancellationToken cancellationToken)
        {
            Command command = await _dbContext.Commands.FirstOrDefaultAsync(c => c.CommandName == request.CommandName, cancellationToken);

            if (command == null)
            {
                return new()
                {
                    CommandWasEdited = false
                };
            }

            command.CommandOutput = request.CommandOutput;
            _ = _dbContext.SaveChanges();

            return new()
            {
                CommandWasEdited = true
            };
        }
    }
}