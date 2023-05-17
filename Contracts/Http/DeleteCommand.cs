using System.ComponentModel.DataAnnotations;

namespace Contracts.Http
{
    public class DeleteCommandRequest
    {
        [Required]
        public string CommandName { get; init; }
    }

    public class DeleteCommandResponse
    {
        public bool DeleteIsSuccessful { get; init; }
    }
}