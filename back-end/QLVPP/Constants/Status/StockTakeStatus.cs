namespace QLVPP.Constants.Status
{
    public class StockTakeStatus
    {
        public const string Pending = "PENDING";
        public const string Approve = "APPROVED";
        public const string Cancelled = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending,
            Approve,
            Cancelled,
        };
    }
}
