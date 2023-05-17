using System.ComponentModel.DataAnnotations;

namespace Contracts.Http
{
    public class DeleteSocialMediaRequest
    {
        [Required]
        public string SocialNetworkName { get; init; }
    }

    public class DeleteSocialMediaResponse
    {
        public bool DeleteIsSuccessful { get; init; }
    }
}