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
    public class GetAllSocialMediasQuery : IRequest<GetAllSocialMediasQueryResult>
    {
    }

    public class GetAllSocialMediasQueryResult
    {
        public ICollection<SocialMedia> SocialMedias { get; set; }
    }

    internal class GetAllSocialMediasQueryHandler : BaseHandler<GetAllSocialMediasQuery, GetAllSocialMediasQueryResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public GetAllSocialMediasQueryHandler(TwitchBotDbContext dbContext, ILogger<GetAllSocialMediasQuery> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<GetAllSocialMediasQueryResult> HandleInternal(GetAllSocialMediasQuery request, CancellationToken cancellationToken)
        {
            List<SocialMedia> socialMedias = await _dbContext.SocialMedias.ToListAsync(cancellationToken);

            return new()
            {
                SocialMedias = socialMedias
            };
        }
    }
}