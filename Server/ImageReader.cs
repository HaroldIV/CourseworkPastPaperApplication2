using System.IO;
using Tesseract;
using System.Threading.Tasks;
using System;

namespace CourseworkPastPaperApplication2.Server
{
    // Class facade for the Tesseract OCR. 
    public static class ImageReader
    {
        // Uses a cached TesseractEngine to save on memory. 
        private static readonly TesseractEngine ocr;
        // Since uses a cached engine, needs a boolean flag to keep track of whether or not the engine is currently in use. 
        private static bool isProcessing;

        // Initialises the cached engine with the tesseract data and english as the language. 
        static ImageReader()
        {
            string dataPath = """C:\Users\jeff1\source\repos\CourseworkPastPaperApplication2\Server\tessdata\""";
            ocr = new TesseractEngine(dataPath, "eng", EngineMode.Default);
        }

        // Reads the image and returns the string form. 
        public static async Task<string> ReadImage(byte[] imageBytes)
        {
            // Loads the image from memory. 
            using Pix image = Pix.LoadFromMemory(imageBytes);
         
            // Waits half a second until the engine is not in use to continue.
            // The half-second wait time is to avoid flooding the server with read instructions. 
            while (isProcessing)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            // sets the isProcessing boolean flag to indicate that the engine is in use and cannot be used. 
            isProcessing = true;
            string returnValue;
            // processes and reads the page. 
            using (Page page = ocr.Process(image))
            {
                returnValue = page.GetText();
            }
            
            // Indicates that the engine is free to use again and returns
            isProcessing = false;
            return returnValue;
        }

        // Runs the ReadImage function without blocking the thread so the server can do other things at the same time with that same thread. 
        public static async Task<string> ReadImageAsync(byte[] imageBytes)
        {
            return await Task.Run(() => ReadImage(imageBytes));
        }
    }
}
