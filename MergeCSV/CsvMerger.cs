using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace MergeCSV
{
    public class CsvMerger
    {

        private DataTable _originalFile = new DataTable();
        private DataTable _secondFile = new DataTable();

        public CsvMerger(DataTable originalFile, DataTable secondFile)
        {
            this.OriginalFile = originalFile;
            this.SecondFile = secondFile;

            //Merges to dataTables 
            OriginalFile.Merge(SecondFile);

            prepareTable();
        }

        //Function that checks if any row repeats in DataTable by comaprising dimensions
        //if yes it returns List of indexes of this rows
        List<int> checkIfThisRowRepeats(DataTable dataTable, DataRow dataRow)
        {
            int i;
            int numberOfRows = dataTable.Rows.Count;
            
            List<int> indexesOfDimensions = new List<int>();
            List<int> indexesOfSameValueRows = new List<int>();

            //Gets indexes of dimensions by looking for letters in their values
            foreach (DataRow row in dataTable.Rows)
            {
                for (i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (row.ItemArray[i].ToString().Any(char.IsLetter))
                        if (!indexesOfDimensions.Contains(i))
                            indexesOfDimensions.Add(i);
                }
            }

            for (i = 0; i < numberOfRows; i++)
            {
                int counter = 0;

                foreach (int index in indexesOfDimensions)
                {

                    if (dataTable.Rows[i].ItemArray[index].ToString() == dataRow.ItemArray[index].ToString())
                    {
                        counter = counter + 1;

                        if (counter == indexesOfDimensions.Count())
                            indexesOfSameValueRows.Add(i);
                    }
                }               
            }

            if (indexesOfSameValueRows.Count() > 1) { return indexesOfSameValueRows; }
            return null;
        }

        //Function that combines data of rows with the same dimensions (it gets indexes from "checkIfThisRowRepeats" function)
        //It places all data from the same rows in the first one on the list with selected dimensions
        //After creating row with all combined data, it deletes rows that repeat (from whom it took data earlier)
        void prepareTable()
        {
            int i;
            int n = OriginalFile.Rows.Count;
            List<int> rowsToDelete = new List<int>();

            foreach (DataColumn column in OriginalFile.Columns) column.ReadOnly = false;

            for (i = 0; i < n; i++)
            {
                DataRow currentRow = OriginalFile.Rows[i];

                List<int> sameCustomerAndProductRowIndex = checkIfThisRowRepeats(OriginalFile, currentRow);

                if (sameCustomerAndProductRowIndex != null)
                {
                    int j;
                 
                    for (j = 0; j < OriginalFile.Columns.Count; j++)
                    {
                        if (currentRow.ItemArray[j].ToString() == "")
                        {
                            foreach(int index in sameCustomerAndProductRowIndex)
                            {
                                if (OriginalFile.Rows[index].ItemArray[j].ToString() != "")
                                {
                                    currentRow.SetField(j, OriginalFile.Rows[index].ItemArray[j].ToString());
                                    break;
                                }
                            }
                        }
                    }

                    sameCustomerAndProductRowIndex.RemoveAt(0);

                    foreach (int index in sameCustomerAndProductRowIndex)
                        if(!rowsToDelete.Contains(index))
                            rowsToDelete.Add(index);
                }
            }

            foreach (int index in rowsToDelete)
                OriginalFile.Rows[index].Delete();

            OriginalFile.AcceptChanges();
        }



        public DataTable OriginalFile { get => _originalFile; set => _originalFile = value; }
        public DataTable SecondFile { get => _secondFile; set => _secondFile = value; }
    }
}
