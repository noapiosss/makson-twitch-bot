using System.ComponentModel.DataAnnotations;

namespace Contracts.Http
{
    public class AddSocialMediaRequest
    {
        [Required]
        public string SocialNetworkName { get; init; }

        [Required]
        public string Link { get; init; }
    }

    public class AddSocialMediaResponse
    {
        public bool CommandIsAdded { get; init; }
    }
}