namespace QLVPP.Constants.Status
{
    public class ReturnStatus
    {
        public const string Pending = "PENDING";
        public const string Approved = "APPROVED";
        public const string Returned = "RETURNED";
        public const string Cancelled = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending,
            Approved,
            Returned,
            Cancelled,
        };
    }
}
