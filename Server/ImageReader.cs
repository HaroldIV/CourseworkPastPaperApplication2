using Syncfusion.Blazor.CircularGauge.Internal;
using System.IO;
using Tesseract;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Server
{
    
    public class ImageReader
    {
        private static readonly TesseractEngine ocr;

        static ImageReader()
        {
            string dataPath = """C:\Users\jeff1\source\repos\CourseworkPastPaperApplication2\Server\tessdata\""";
            ocr = new TesseractEngine(dataPath, "eng", EngineMode.Default);
        }

        public static string ReadImage(byte[] imageBytes)
        {
            using Pix image = Pix.LoadFromMemory(imageBytes);
            
            using Page page = ocr.Process(image);
            
            return page.GetText();
        }

        public static async Task<string> ReadImageAsync(byte[] imageBytes)
        {
            return await Task.Run(() => ReadImage(imageBytes));
        }
    }
}
