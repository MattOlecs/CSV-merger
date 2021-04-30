using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MergeCSV
{
    public static class CsvSaver
    {

        //Its a simple function that converts DataTable into csv file format and saves it in selected path
        //It also includes converting numerics into floating point values with up to 10 fraction digits
        public static void SaveAsCSV(this DataTable dataTable, string strFilePath)
        {
            StreamWriter writer = new StreamWriter(strFilePath, false);
            //headers
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                writer.Write(dataTable.Columns[i]);
                if (i < dataTable.Columns.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.Write(writer.NewLine);
            foreach (DataRow dr in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        double tmp = 0;
                        if(double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                        {
                            Math.Round(tmp,11);
                            value = tmp.ToString().Replace(",",".");
                        }

                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            writer.Write(value);
                        }
                        else
                        {
                            writer.Write(dr[i].ToString());
                        }
                    }
                    if (i < dataTable.Columns.Count - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.Write(writer.NewLine);
            }
            writer.Close();
        }
    }
}
