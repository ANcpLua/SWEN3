using OcrWorker.Tesseract;

namespace OcrWorker;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("write dotnet run -- \"docs/TourPlanner_Specification.pdf\" into the Terminal inside your Paperless/OcrWorker directory");
            return;
        }
       
        string filePath = Path.GetFullPath(args[0]);
        string language = args.Length > 1 ? args[1] : "eng";

        Console.WriteLine($"Processing file: {filePath}");
        Console.WriteLine($"Using language: {language}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"The specified file does not exist: {filePath}");
            return;
        }

        if (language != "eng")
        {
            Console.WriteLine("Invalid language specified. Only 'eng' is supported.");
            return;
        }

        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string tessDataPath = Path.Combine(baseDirectory, "tessdata");
            
            Console.WriteLine($"Using tessdata path: {tessDataPath}");

            if (!Directory.Exists(tessDataPath))
            {
                Console.WriteLine($"Creating tessdata directory at: {tessDataPath}");
                Directory.CreateDirectory(tessDataPath);
            }

            string langFile = Path.Combine(tessDataPath, $"{language}.traineddata");
            if (!File.Exists(langFile))
            {
                Console.WriteLine($"Language file not found at: {langFile}");
                Console.WriteLine("Please download the language file using:");
                Console.WriteLine($"Invoke-WebRequest -Uri \"https://github.com/tesseract-ocr/tessdata/raw/main/{language}.traineddata\" -OutFile \"{langFile}\"");
                return;
            }

            var options = new OcrOptions
            {
                Language = language,
                TessDataPath = tessDataPath
            };

            Console.WriteLine("Initializing OCR client...");
            var ocrClient = new Ocr(options);

            Console.WriteLine("Reading file...");
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                Console.WriteLine("Starting OCR process...");
                var ocrContentText = ocrClient.OcrPdf(fileStream);
                
                Console.WriteLine("\nOCR Result:");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(ocrContentText);
                Console.WriteLine("----------------------------------------");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while processing the file:");
            Console.WriteLine($"Exception Message: {e.Message}");
            Console.WriteLine($"Stack Trace: {e.StackTrace}");
            if (e.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
            }
        }
    }
}