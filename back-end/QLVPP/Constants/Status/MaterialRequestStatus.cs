namespace QLVPP.Constants.Status
{
    public static class MaterialRequestStatus
    {
        public const string Pending_Department = "PENDING_DEPARTMENT";
        public const string Pending_Warehouse = "PENDING_WAREHOUSE";
        public const string Approved = "APPROVED";
        public const string Rejected = "REJECTED";
        public const string Cancelled = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending_Warehouse,
            Pending_Department,
            Approved,
            Rejected,
            Cancelled,
        };
    }
}
