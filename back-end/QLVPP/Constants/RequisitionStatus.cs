namespace QLVPP.Constants
{
    public static class RequisitionStatus
    {
        public const string Draft = "DRAFT";
        public const string Pending = "PENDING";
        public const string Approved = "APPROVED";
        public const string Rejected = "REJECTED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Draft,
            Pending,
            Approved,
            Rejected,
        };
    }
}
