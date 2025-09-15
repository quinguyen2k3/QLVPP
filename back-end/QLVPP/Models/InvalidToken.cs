using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("InvalidToken")]
    public class InvalidToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Jti { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public DateTime RevokedAt { get; set; }
        public string RevokedBy { get; set; } = string.Empty;
    }
}
