using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Domain.Database;
using MediatR;
using Domain.Base;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Domain.Queries
{
    public class GetCommandsByTypeQuery : IRequest<GetCommandsByTypeQueryResult>
    {
        public CommandType CommadType { get; set; }
    }

    public class GetCommandsByTypeQueryResult
    {
        public ICollection<Command> Commands { get; set; }
    }

    internal class GetCommandsByTypeQueryHandler : BaseHandler<GetCommandsByTypeQuery, GetCommandsByTypeQueryResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public GetCommandsByTypeQueryHandler(TwitchBotDbContext dbContext, ILogger<GetCommandsByTypeQuery> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<GetCommandsByTypeQueryResult> HandleInternal(GetCommandsByTypeQuery request, CancellationToken cancellationToken)
        {
            List<Command> commands = await _dbContext.Commands.Where(c => c.CommandType == request.CommadType).ToListAsync(cancellationToken);

            return new()
            {
                Commands = commands
            };
        }
    }
}