namespace QLVPP.Constants.Types
{
    public static class ApprovalType
    {
        public const string SEQUENTIAL = "SEQUENTIAL";
        public const string PARALLEL = "PARALLEL";

        public static readonly HashSet<string> All = new HashSet<string> { SEQUENTIAL, PARALLEL };
    }
}
