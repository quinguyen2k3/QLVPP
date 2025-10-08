using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ProductReq
    {
        [Required(ErrorMessage = "Product Code is required")]
        public string ProdCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "UnitId is required")]
        public long UnitId { get; set; }
        [Required(ErrorMessage = "CategoryId is required")]
        public long CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Depth { get; set; }
        [Required(ErrorMessage = "IsAsset status is required")]
        public bool IsAssest { get; set; } 
        [Required(ErrorMessage = "IsActived status is required")]
        public bool IsActived { get; set; }
        public long WarehouseId { get; set; }
    }
}
