using Dynamsoft.Core;
using Dynamsoft.CVR;
using Dynamsoft.DLR;
using Dynamsoft.License;

namespace RecognizeAnImage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int errorCode = 0;
            string errorMsg;
            errorCode = LicenseManager.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9", out errorMsg);
            if(errorCode!=(int)EnumErrorCode.EC_OK)
            {
                Console.WriteLine("License initialization: " + errorCode + "," + errorMsg);
            }

            using (CaptureVisionRouter cvr = new CaptureVisionRouter())
            {
                string imageFile = "../../../../../../Images/dlr-sample.png";
                CapturedResult? result = cvr.Capture(imageFile, PresetTemplate.PT_RECOGNIZE_TEXT_LINES);
                Console.WriteLine("File: " + imageFile);
                if (result == null)
                {
                    Console.WriteLine("No Result.");
                }
                else if (result.GetErrorCode() != 0)
                {
                    Console.WriteLine("Error: " + result.GetErrorCode() + "," + result.GetErrorString());
                }
                else
                {
                    RecognizedTextLinesResult? textLinesResult = result.GetRecognizedTextLinesResult();
                    if (textLinesResult != null)
                    {
                        TextLineResultItem[] items = textLinesResult.GetItems();
                        Console.WriteLine("Recognized " + items.Length + " text lines");
                        foreach (TextLineResultItem textLineItem in items)
                        {
                            Console.WriteLine(">>Line result : " + Array.IndexOf(items, textLineItem) + ": " + textLineItem.GetText());
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to quit...");
            Console.Read();
        }
    }
}