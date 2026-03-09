namespace QLVPP.DTOs.Response
{
    public class PositionRes
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsActivated { get; set; }
    }
}
