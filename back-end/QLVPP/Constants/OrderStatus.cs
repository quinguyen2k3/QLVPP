namespace QLVPP.Constants
{
    public class OrderStatus
    {
        public const string Pending = "PENDING";
        public const string PartiallyReceived = "PARTIALLY-RECEIVED";
        public const string Complete = "COMPLETE";
        public const string Cancelled = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending,
            PartiallyReceived,
            Complete,
            Cancelled,
        };
    }
}
