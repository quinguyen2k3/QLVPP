using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class DelegateReq
    {
        [Required(ErrorMessage = "RequestId is required")]
        public long RequestId { get; set; }

        [Required(ErrorMessage = "DelegateToId is required")]
        public long DelegateToId { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }
    }
}
