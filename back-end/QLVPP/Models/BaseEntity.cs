using System;
using System.ComponentModel.DataAnnotations;

namespace QLVPP.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActived { get; set; }

        public BaseEntity()
        {
            CreatedDate = DateTime.Now;
            IsActived = true;
        }
    }
}
