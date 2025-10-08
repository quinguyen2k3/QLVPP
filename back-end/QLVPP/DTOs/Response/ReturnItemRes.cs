using QLVPP.Models;

namespace QLVPP.DTOs.Response
{
    public class ReturnItemRes
    {
        public long AssetLoanId { get; set; }
        public int ReturnedQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        public string? Note { get; set; }
    }
}