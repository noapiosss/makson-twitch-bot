using System.Collections.Generic;
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
    public class GetAllCommandsQuery : IRequest<GetAllCommandsQueryResult>
    {
    }

    public class GetAllCommandsQueryResult
    {
        public ICollection<Command> Commands { get; set; }
    }

    internal class GetAllCommandsQueryHandler : BaseHandler<GetAllCommandsQuery, GetAllCommandsQueryResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public GetAllCommandsQueryHandler(TwitchBotDbContext dbContext, ILogger<GetAllCommandsQuery> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<GetAllCommandsQueryResult> HandleInternal(GetAllCommandsQuery request, CancellationToken cancellationToken)
        {
            List<Command> commands = await _dbContext.Commands.ToListAsync(cancellationToken);

            return new()
            {
                Commands = commands
            };
        }
    }
}