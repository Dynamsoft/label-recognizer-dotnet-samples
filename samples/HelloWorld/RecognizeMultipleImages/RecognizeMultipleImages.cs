using Dynamsoft.Core;
using Dynamsoft.CVR;
using Dynamsoft.DLR;
using Dynamsoft.License;
using Dynamsoft.Utility;
namespace RecognizeMultipleImages
{
    class MyCapturedResultReceiver : CapturedResultReceiver
    {
        public override void OnRecognizedTextLinesReceived(RecognizedTextLinesResult result)
        {
            FileImageTag? tag = (FileImageTag?)result.GetOriginalImageTag();
            Console.WriteLine("File: " + tag.GetFilePath());
            if (result.GetErrorCode() != (int)EnumErrorCode.EC_OK)
            {
                Console.WriteLine("Error: " + result.GetErrorString());
            }
            else
            {
                TextLineResultItem[] items = result.GetItems();
                Console.WriteLine("Recognized " + items.Length + " text lines");
                foreach (TextLineResultItem item in items)
                {
                    Console.WriteLine(">>Line result : " + Array.IndexOf(items, item) + ": " + item.GetText());
                }
            }
            Console.WriteLine();
        }
    }
    class MyImageSourceStateListener : IImageSourceStateListener
    {
        private CaptureVisionRouter? cvr = null;
        public MyImageSourceStateListener(CaptureVisionRouter cvr)
        {
            this.cvr = cvr;
        }

        public void OnImageSourceStateReceived(EnumImageSourceState state)
        {
            if (state == EnumImageSourceState.ISS_EXHAUSTED)
            {
                if (cvr != null)
                {
                    cvr.StopCapturing();
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int errorCode = 1;
            string errorMsg;
            errorCode = LicenseManager.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9", out errorMsg);
            if (errorCode != (int)EnumErrorCode.EC_OK)
            {
                Console.WriteLine("License initialization error: " + errorMsg);
            }
            using (CaptureVisionRouter cvr = new CaptureVisionRouter())
            using (DirectoryFetcher fetcher = new DirectoryFetcher())
            {
                fetcher.SetDirectory("../../../../../../Images");
                cvr.SetInput(fetcher);

                CapturedResultReceiver receiver = new MyCapturedResultReceiver();
                cvr.AddResultReceiver(receiver);

                MyImageSourceStateListener listener = new MyImageSourceStateListener(cvr);
                cvr.AddImageSourceStateListener(listener);

                errorCode = cvr.StartCapturing(PresetTemplate.PT_RECOGNIZE_TEXT_LINES, true, out errorMsg);
                if (errorCode != (int)EnumErrorCode.EC_OK)
                {
                    Console.WriteLine("error: " + errorMsg);
                }
            }
            Console.WriteLine("Press any key to quit...");
            Console.Read();
        }
    }
}