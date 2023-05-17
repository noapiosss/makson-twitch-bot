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
    public class DeleteSocialMedia : IRequest<DeleteSocialMediaResult>
    {
        public string SocialNetworkName { get; set; }
    }

    public class DeleteSocialMediaResult
    {
        public bool SocialMediaIsDeleted { get; set; }
    }

    internal class DeleteSocialMediaHandler : BaseHandler<DeleteSocialMedia, DeleteSocialMediaResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public DeleteSocialMediaHandler(TwitchBotDbContext dbContext, ILogger<DeleteSocialMedia> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<DeleteSocialMediaResult> HandleInternal(DeleteSocialMedia request, CancellationToken cancellationToken)
        {
            request.SocialNetworkName = request.SocialNetworkName.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            SocialMedia socialMedia = await _dbContext.SocialMedias.FirstOrDefaultAsync(sm => sm.SocialNetworkName == request.SocialNetworkName, cancellationToken);

            if (socialMedia == null)
            {
                return new()
                {
                    SocialMediaIsDeleted = false
                };
            }


            _ = _dbContext.SocialMedias.Remove(socialMedia);
            _ = _dbContext.SaveChanges();

            return new()
            {
                SocialMediaIsDeleted = true
            };
        }
    }
}