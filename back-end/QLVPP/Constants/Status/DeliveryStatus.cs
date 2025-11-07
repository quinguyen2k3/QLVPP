namespace QLVPP.Constants.Status
{
    public class DeliveryStatus
    {
        public const string Pending = "PENDING";
        public const string Complete = "COMPLETE";

        public static readonly HashSet<string> All = new HashSet<string> { Pending, Complete };
    }
}
