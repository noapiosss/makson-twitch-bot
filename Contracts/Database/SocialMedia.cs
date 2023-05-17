using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Database
{
    [Table("tbl_social_media")]
    public class SocialMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("social_network")]
        public string SocialNetworkName { get; set; }

        [Required]
        [Column("link")]
        public string Link { get; set; }
    }
}