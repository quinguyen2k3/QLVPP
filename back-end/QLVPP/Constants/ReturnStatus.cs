namespace QLVPP.Constants
{
    public class ReturnStatus
    {
        public const string Pending = "PENDING";
        public const string Complete = "COMPLETED";
        public const string Cancel = "CANCELLED";

        public static readonly HashSet<string> All = new HashSet<string>
        {
            Pending,
            Complete,
            Cancel,
        };
    }
}
