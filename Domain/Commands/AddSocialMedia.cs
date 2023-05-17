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
    public class AddSocialMedia : IRequest<AddSocialMediaResult>
    {
        public SocialMedia SocialMedia { get; set; }
    }

    public class AddSocialMediaResult
    {
        public bool SocialMediaIsAdded { get; set; }
    }

    internal class AddSocialMediaHandler : BaseHandler<AddSocialMedia, AddSocialMediaResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public AddSocialMediaHandler(TwitchBotDbContext dbContext, ILogger<AddSocialMedia> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<AddSocialMediaResult> HandleInternal(AddSocialMedia request, CancellationToken cancellationToken)
        {
            request.SocialMedia.SocialNetworkName = request.SocialMedia.SocialNetworkName.ToLower(System.Globalization.CultureInfo.CurrentCulture);

            if (await _dbContext.SocialMedias.AnyAsync(sm => sm.SocialNetworkName == request.SocialMedia.SocialNetworkName, cancellationToken))
            {
                return new()
                {
                    SocialMediaIsAdded = false
                };
            }

            _ = await _dbContext.AddAsync(request.SocialMedia, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return new()
            {
                SocialMediaIsAdded = true
            };
        }
    }
}