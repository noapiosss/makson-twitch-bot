using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Database
{
    [Table("tbl_commands")]
    public class Command
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("command_name")]
        public string CommandName { get; set; }

        [Required]
        [Column("command_output")]
        public string CommandOutput { get; set; }
    }
}