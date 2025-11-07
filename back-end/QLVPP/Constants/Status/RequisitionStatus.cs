namespace QLVPP.Constants.Status
{
    public static class RequisitionStatus
    {
        public const string Pending = "PENDING";
        public const string Approved = "APPROVED";
        public const string Rejected = "REJECTED";
        public const string Cancelled = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending,
            Approved,
            Rejected,
            Cancelled,
        };
    }
}
