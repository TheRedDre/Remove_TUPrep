using System;
using System.Collections.Generic;
using System.IO;

namespace Remove_TUPrep
{
    class Program
    {

        private static string _fileName = "Druzhba.txt";

        // Для удаления
        private static List<string> _luNames = new List<string>()
        {
            "LU_NIKOL1",
            "LU_KLIN",
            "LU_KIJEV",
            "LU_SOSED",
            "LU_KUZN"
        };

        private static void Main()
        {
            int countRemoved = 0;
            int countAllBefore = 0;
            int countAllAfter = 0;
            int countAdded = 0;

            var outputFile = new StreamWriter("reworked_" + _fileName, false);

            // Удаление TUPrep
            var allLines = File.ReadAllLines(_fileName);
            foreach (var line in allLines)
            {
                string[] lineParts = line.Split('\t');
                if (!string.IsNullOrEmpty(lineParts[8]))
                    countAllBefore++;
                if (lineParts[8].ToUpper().Contains("TUPREP") && (lineParts[8].ToUpper().Contains(_luNames[0])
                                                                  || lineParts[8].ToUpper().Contains(_luNames[1])
                                                                  || lineParts[8].ToUpper().Contains(_luNames[2])
                                                                  || lineParts[8].ToUpper().Contains(_luNames[3])
                                                                  || lineParts[8].ToUpper().Contains(_luNames[4]))
                                                              && !lineParts[8].ToUpper().Contains("NPS_"))
                {
                    lineParts[8] = string.Empty;
                    countRemoved++;
                }

                foreach (var linePart in lineParts)
                {
                    outputFile.Write(linePart + "\t");
                }

                outputFile.WriteLine();
            }
            outputFile.Close();
            //Console.WriteLine($"Removed: {countRemoved} from {countAll} tags");

            // Добавление TUPrep ко всем остальным
            var reworkedLines = File.ReadAllLines("reworked_" + _fileName);
            var outputFile2 = new StreamWriter("druzhba_good.txt", false);
            foreach (var line in reworkedLines)
            {
                string[] lineParts = line.Split('\t');
                // Вытаскиваем тег на задвижку
                string tagName = string.Empty;
                if (lineParts[3].Equals("Valve") && (!lineParts[1].Contains("МНС") || !lineParts[1].Contains("УРД") ||
                                                     !lineParts[1].Contains("НПС") || !lineParts[1].Contains("РП") ||
                                                     !lineParts[1].Contains("ПНС")) && lineParts[6].Equals("ID") &&
                    !string.IsNullOrEmpty(lineParts[8]))
                {
                    tagName = lineParts[8];
                    Console.WriteLine(tagName);
                }
                else
                {
                    tagName = string.Empty;
                }
                // Добавляем тег TUPREP на текущее задвижку, если она отвечает условиям отсутствия ЦСПА
                if (!string.IsNullOrEmpty(tagName) && (!tagName.ToUpper().Contains(_luNames[0])
                    || !tagName.ToUpper().Contains(_luNames[1])
                    || !tagName.ToUpper().Contains(_luNames[2])
                    || !tagName.ToUpper().Contains(_luNames[3])
                    || !tagName.ToUpper().Contains(_luNames[4]))
                    && !tagName.ToUpper().Contains("NPS_") && (lineParts[6].Equals("CanBeControlled")))
                {
                    lineParts[8] = tagName + ".TUPREP";
                    countAdded++;
                }


                foreach (var linePart in lineParts)
                {
                    outputFile2.Write(linePart + "\t");
                }
                outputFile2.WriteLine();
            }

            outputFile2.Close();

            var lines = File.ReadAllLines("druzhba_good.txt");
            foreach (var line in lines)
            {
                var lineParts = line.Split('\t');
                if (!string.IsNullOrEmpty(lineParts[8]))
                    countAllAfter++;
            }
            Console.WriteLine($"Total tags count before operations: {countAllBefore}\nTags removed: {countRemoved} added: {countAdded}\nTotal tags after: {countAllAfter}");
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
