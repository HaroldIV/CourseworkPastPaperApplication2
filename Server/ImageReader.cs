using Syncfusion.Blazor.CircularGauge.Internal;
using System.IO;
using Tesseract;
using System.Threading.Tasks;
using System;

namespace CourseworkPastPaperApplication2.Server
{
    
    public class ImageReader
    {
        private static readonly TesseractEngine ocr;
        private static bool isProcessing;

        static ImageReader()
        {
            string dataPath = """C:\Users\jeff1\source\repos\CourseworkPastPaperApplication2\Server\tessdata\""";
            ocr = new TesseractEngine(dataPath, "eng", EngineMode.Default);
        }

        public static async Task<string> ReadImage(byte[] imageBytes)
        {
            using Pix image = Pix.LoadFromMemory(imageBytes);
         
            while (isProcessing)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            isProcessing = true;
            string returnValue;
            using (Page page = ocr.Process(image))
            {
                returnValue = page.GetText();
            }
            
            isProcessing = false;
            return returnValue;
        }

        public static async Task<string> ReadImageAsync(byte[] imageBytes)
        {
            return await Task.Run(() => ReadImage(imageBytes));
        }
    }
}
