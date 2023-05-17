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
    public class EditSocialMedia : IRequest<EditSocialMediaResult>
    {
        public string SocialNetworkName { get; set; }
        public string Link { get; set; }
    }

    public class EditSocialMediaResult
    {
        public bool SocialMediaWasEdited { get; set; }
    }

    internal class EditSocialMediaHandler : BaseHandler<EditSocialMedia, EditSocialMediaResult>
    {
        private readonly TwitchBotDbContext _dbContext;

        public EditSocialMediaHandler(TwitchBotDbContext dbContext, ILogger<EditSocialMedia> logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected override async Task<EditSocialMediaResult> HandleInternal(EditSocialMedia request, CancellationToken cancellationToken)
        {
            request.SocialNetworkName = request.SocialNetworkName.ToLower(System.Globalization.CultureInfo.CurrentCulture);

            SocialMedia socialMedia = await _dbContext.SocialMedias.FirstOrDefaultAsync(sm => sm.SocialNetworkName == request.SocialNetworkName, cancellationToken);

            if (socialMedia == null)
            {
                return new()
                {
                    SocialMediaWasEdited = false
                };
            }

            socialMedia.Link = request.Link;
            _ = _dbContext.SaveChanges();

            return new()
            {
                SocialMediaWasEdited = true
            };
        }
    }
}