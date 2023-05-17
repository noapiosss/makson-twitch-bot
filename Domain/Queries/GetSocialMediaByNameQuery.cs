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
    public class GetSocialMediaByNameQuery : IRequest<GetSocialMediaByNameQueryResult>
    {
        public string SocialNetworkName { get; set; }
    }

    public class GetSocialMediaByNameQueryResult
    {
        public SocialMedia SocialMedia { get; set; }
    }

    internal class GetSocialMediaByNameQueryHandler : BaseHandler<GetSocialMediaByNameQuery, GetSocialMediaByNameQueryResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public GetSocialMediaByNameQueryHandler(TwitchBotDbContext dbContext, ILogger<GetSocialMediaByNameQuery> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<GetSocialMediaByNameQueryResult> HandleInternal(GetSocialMediaByNameQuery request, CancellationToken cancellationToken)
        {
            request.SocialNetworkName = request.SocialNetworkName.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            SocialMedia socialMedia = await _dbContext.SocialMedias.FirstOrDefaultAsync(sm => sm.SocialNetworkName == request.SocialNetworkName, cancellationToken);

            return new()
            {
                SocialMedia = socialMedia
            };
        }
    }
}