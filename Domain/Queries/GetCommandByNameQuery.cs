using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Domain.Database;
using MediatR;
using Domain.Base;
using Microsoft.Extensions.Logging;

namespace Domain.Queries
{
    public class GetCommandByNameQuery : IRequest<GetCommandByNameQueryResult>
    {
        public string CommandName { get; set; }
    }

    public class GetCommandByNameQueryResult
    {
        public Command Command { get; set; }
    }

    internal class GetCommandByNameQueryHandler : BaseHandler<GetCommandByNameQuery, GetCommandByNameQueryResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public GetCommandByNameQueryHandler(TwitchBotDbContext dbContext, ILogger<GetCommandByNameQuery> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<GetCommandByNameQueryResult> HandleInternal(GetCommandByNameQuery request, CancellationToken cancellationToken)
        {
            request.CommandName = request.CommandName.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            Command command = await _dbContext.Commands.FirstOrDefaultAsync(c => c.CommandName == request.CommandName, cancellationToken);

            return new()
            {
                Command = command
            };
        }
    }
}