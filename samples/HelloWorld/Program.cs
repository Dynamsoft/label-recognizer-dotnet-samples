using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dynamsoft.DLR;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
		// 1.Initialize license.
		// The string "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9" here is a 7-day free license. Note that network connection is required for this license to work.
		// If you want to use an offline license, please contact Dynamsoft Support: https://www.dynamsoft.com/company/contact/
		// You can also request a 30-day trial license in the customer portal: https://www.dynamsoft.com/customer/license/trialLicense?product=dlr&utm_source=github&package=dotnet
                LabelRecognizer.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInByb2R1Y3RzIjoyfQ==");

                // 2.Create an instance of Label Recognizer.
                LabelRecognizer dlr = new LabelRecognizer();

                DLR_Result[] results = null;

                try
                {
                    // 3.Recognize text from an image file.
                    results = dlr.RecognizeByFile("../../../../images/dlr-sample-vin.png", "");
                }
                catch (LabelRecognizerException exp)
                {
                    Console.WriteLine(exp);
                }

                if (results != null && results.Length > 0)
                {
                    for (int i = 0; i < results.Length; ++i)
                    {
                        Console.WriteLine("Result " + i.ToString() + ":");

                        // Get result of each text area (also called label).
                        DLR_Result result = results[i];
                        for (int j = 0; j < result.LineResults.Length; ++j)
                        {
                            // Get the result of each text line in the label.
                            DLR_LineResult lineResult = result.LineResults[j];
                            Console.WriteLine(">>LineResult " + j.ToString() + ": " + lineResult.Text);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No data detected.\n");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
