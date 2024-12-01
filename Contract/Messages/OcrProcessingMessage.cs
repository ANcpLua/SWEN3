using System;

namespace Contract.Messages;

public class OcrProcessingMessage
{
    public string DocumentId { get; set; }
    public string FilePath { get; set; }
    public DateTime Timestamp { get; set; }
    public string OriginalFileName { get; set; }
}
