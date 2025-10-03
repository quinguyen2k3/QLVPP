using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Response
{
    public class ProductRes
    {
        public long Id { get; set; }
        public string ProdCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long UnitId { get; set; }
        public long CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Depth { get; set; }
        public bool IsActived { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
