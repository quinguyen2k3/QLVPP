namespace QLVPP.Constants.Status
{
    public class StockOutStatus
    {
        public const string Pending = "PENDING";
        public const string Approved = "APPROVED";
        public const string Received = "RECEIVED";
        public const string Cancelled = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending,
            Approved,
            Received,
            Cancelled,
        };
    }
}
