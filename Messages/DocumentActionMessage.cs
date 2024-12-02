
namespace Messages
{
    public class DocumentActionMessage
    {
        public int DocumentId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}