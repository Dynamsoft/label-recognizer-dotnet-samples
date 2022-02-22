using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Dynamsoft.Core;
using Dynamsoft.DLR;

namespace PassportMRZReading
{
    class Program
    {
        static bool GetImagePath(out string imagePath)
        {
            imagePath = "";
            bool bExit = false;
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(">> Step 1: Input your image file's full path:");
                imagePath = Console.ReadLine();
                if (imagePath == "Q" || imagePath == "q")
                {
                    bExit = true;
                    break;
                }
                try
                {
                    int tempIndex = imagePath.IndexOf(' ');
                    if (tempIndex != -1)
                    {
                        imagePath = imagePath.Substring(1, imagePath.Length - 2);
                    }
                    bool isFileExist = File.Exists(imagePath);
                    if (isFileExist)
                    {
                        bExit = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("The file is not found. Please input a valid path.");
                        continue;
                    }
                }
                catch (FileNotFoundException exp)
                {
                    Console.WriteLine(exp);
                    continue;
                }
            }

            return bExit;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("*************************************************");
            Console.WriteLine("Welcome to Dynamsoft Label Recognizer - Passport MRZ Sample");
            Console.WriteLine("*************************************************");
            Console.WriteLine("Hints: Please input 'Q' or 'q' to quit the application.");

            try
            {

                // 1.Initialize license.
                // The string "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9" here is a free public trial license. Note that network connection is required for this license to work.
                // If you want to use an offline license, please contact Dynamsoft Support: https://www.dynamsoft.com/company/contact/
                // You can also request a 30-day trial license in the customer portal: https://www.dynamsoft.com/customer/license/trialLicense?product=dlr&utm_source=github&package=dotnet
                LabelRecognizer.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9");

                // 2. Create an instance of Label Recognizer.
                LabelRecognizer labelRecognizer = new LabelRecognizer();

                // 3. Append config by a template json file.
                labelRecognizer.AppendSettingsFromFile("wholeImgMRZTemplate.json");

                while (true)
                {
                    string imagePath = "";
                    bool bExit = GetImagePath(out imagePath);
                    if (bExit)
                        break;

                    // 4. Recognize text from the image file. The second parameter is set to "locr" which is defined in the template json file.
                    DLR_Result[] results = labelRecognizer.RecognizeByFile(imagePath, "locr");

                    if (results.Length == 0)
                    {
                        Console.WriteLine("No data detected.");
                    }
                    else
                    {
                        // 5. Output the raw text of MRZ.
                        for (int i = 0; i < results.Length; ++i)
                        {
                            Console.WriteLine("Result " + i.ToString() + ": ");
                            for (int j = 0; j < results[i].LineResults.Length; ++j)
                            {
                                Console.WriteLine(">>LineResult " + j.ToString() + ": " + results[i].LineResults[j].Text);
                            }
                        }

                        // 6. Parse the raw text of MRZ and output passport info.
                        Console.WriteLine("Passport Info : ");
                        for (int i = 0; i < results.Length; ++i)
                        {
                            int linesCount = results[i].LineResults.Length;
                            if (linesCount < 2)
                                continue;

                            string line1 = results[i].LineResults[0].Text;
                            string line2 = results[i].LineResults[1].Text;

                            if (line1.Length != 44 || line2.Length != 44)
                                continue;
                            if (line1[0] != 'P')
                                continue;

                            string tmpString = line1.Substring(5);

                            int pos = tmpString.IndexOf("<<");

                            string surname = tmpString.Substring(0, pos);

                            string givenName = tmpString.Substring(pos + 2);

                            string[] surnames = surname.Split('<');

                            surname = "";
                            for (int j = 0; j < surnames.Length; ++j)
                            {
                                if (surnames[j].Length != 0)
                                {
                                    surname = surname + surnames[j] + " ";
                                }
                            }
                            surname = surname.Remove(surname.Length - 1, 1);

                            Console.Write("\tSurname : ");

                            Console.WriteLine(surname);

                            string[] givenNames = givenName.Split('<');

                            givenName = "";
                            for (int j = 0; j < givenNames.Length; ++j)
                            {
                                if (givenNames[j].Length != 0)
                                {
                                    givenName = givenName + givenNames[j] + " ";
                                }
                            }
                            givenName = givenName.Remove(givenName.Length-1, 1);

                            Console.Write("\tGiven Names : ");

                            Console.WriteLine(givenName);

                            string nation = line1.Substring(2, 3);
                            Console.WriteLine("\tNationality : " + nation);

                            tmpString = line2.Substring(0, 9);
                            pos = tmpString.IndexOf('<');
                            if (pos > 0)
                            {
                                tmpString = tmpString.Substring(0, pos);
                            }
                            Console.WriteLine("\tPassport Number : " + tmpString);

                            tmpString = line2.Substring(10, 3);
                            if (tmpString.EndsWith("<"))
                            {
                                tmpString = tmpString.Substring(0, 2);
                            }
                            Console.WriteLine("\tIssuing Country or Organization: " + tmpString);

                            tmpString = line2.Substring(13, 6);
                            tmpString = tmpString.Insert(2, "-");
                            tmpString = tmpString.Insert(5, "-");
                            Console.WriteLine("\tDate of Birth(YY-MM-DD) : " + tmpString);

                            //if (line2[20] == 'F')
                            //    tmpString = "Female";
                            //if (line2[20] == 'M')
                            //    tmpString = "Male";
                            Console.WriteLine("\tSex/Gender : " + line2[20]);

                            tmpString = line2.Substring(21, 6);
                            tmpString = tmpString.Insert(2, "-");
                            tmpString = tmpString.Insert(5, "-");
                            Console.WriteLine("\tPassport Expiration Date(YY-MM-DD) : " + tmpString);

                        }
                    }
                }
            }
            catch (LabelRecognizerException exp)
            {
                Console.WriteLine(exp);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }
    }
}
