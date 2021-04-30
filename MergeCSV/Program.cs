using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace MergeCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathOriginal;
            string pathSecond;


            DataTable csvTableOriginalFile = new DataTable();
            DataTable csvTableSecondFile = new DataTable();

            try
            {
                if (args.Length != 2)
                {
                    throw new ArgumentException("You have to provide two file names.");
                }
                else
                {
                    pathOriginal = args[0];
                    pathSecond = args[1];
                }

                //Those two functions load csv file into DataTable
                //Uses LumenWorks.Framework.IO library
                using (CsvReader csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(pathOriginal))))
                {
                    csvTableOriginalFile.Load(csvReader);
                }

                using (CsvReader csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(pathSecond))))
                {
                    csvTableSecondFile.Load(csvReader);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            CsvMerger merger = new CsvMerger(csvTableOriginalFile, csvTableSecondFile);

            CsvSaver.SaveAsCSV(merger.OriginalFile, "Merged.csv");
        }
    }
}
