using System.ComponentModel.DataAnnotations;

namespace Contracts.Http
{
    public class AddCommandRequest
    {
        [Required]
        public string CommandName { get; init; }

        [Required]
        public string CommandType { get; init; }

        [Required]
        public string CommandOutput { get; init; }
    }

    public class AddCommandResponse
    {
        public bool CommandIsAdded { get; init; }
    }
}