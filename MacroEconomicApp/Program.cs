using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using ClosedXML.Excel;
using CsvHelper;
using System.Globalization;

namespace MacroEconomicApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string csv_file_path = @"C:\Assignment\WDI_CSV\WDIData.csv";
            string tableName = "EconomicIndicators";
            string[] selectedColumns = { "indicator_name", "indicator_code" };
            // Place MacroEconomic_Analysis_DB with the desired path and update below path
            SQLiteConnector connector = new SQLiteConnector("C:\\Users\\ashni\\OneDrive\\Desktop\\MCB\\New folder\\MacroEconomic_Analysis_DB");

            //DataTable csvData = GetDataTabletFromCSVFile(csv_file_path); 
            // Read data from CSV and insert into the existing SQLite table
            //InsertDataFromCsv(tableName, csv_file_path, selectedColumns); 
            // Example: Select data from the table 
            string selectDataQuery = "SELECT * FROM EconomicIndicators";
            // DataTable resultTable = connector.ExecuteQuery(selectDataQuery);

            // Read data from CSV
            List<MyClass> records = ReadCsv<MyClass>(csv_file_path);

            // Assuming all columns are of type TEXT for simplicity
            string insertSql = $@"INSERT INTO {tableName} ({string.Join(",", selectedColumns)}) VALUES ";


            // Print the records to the console for demonstration
            foreach (var record in records)
            {
                Console.WriteLine($"indicator_name: {record.indicator_name}, indicator_code: {record.indicator_code}");

                // Concatenate values to the insert SQL statement
                insertSql += $"('{record.indicator_name}', '{record.indicator_code}'),";
            }
            // Remove the trailing comma
            insertSql = insertSql.TrimEnd(',');
            connector.ExecuteNonQuery(insertSql);
        }

        static List<T> ReadCsv<T>(string csvFilePath)
        {
            using (var reader = new CsvReader(new System.IO.StreamReader(csvFilePath), CultureInfo.InvariantCulture))
            {
                return reader.GetRecords<T>().ToList();
            }
        }

























        public static void InsertDataFromCsv(string tableName, string csvFilePath, string[] selectedColumns)
        {
            // Place MacroEconomic_Analysis_DB with the desired path and update below path
            SQLiteConnector connector = new SQLiteConnector("C:\\Users\\ashni\\OneDrive\\Desktop\\MCB\\New folder\\MacroEconomic_Analysis_DB");

            // Read CSV file and replace commas with semicolons
            string[] lines = File.ReadAllLines(csvFilePath);

            string concatenatedLines = string.Join(";", lines);



            // Assume the first line is the header
            string[] header = lines.First().Split(';');
            string[] headers = header.First().Split(',');

            // Get the indices of selected columns
            var columnIndexMapping = headers.Select((header, index) => new { Header = header, Index = index })
                                           .Where(x => selectedColumns.Contains(x.Header))
                                           .ToDictionary(x => x.Header, x => x.Index);

            // Assuming all columns are of type TEXT for simplicity
            string insertSql = $@"INSERT INTO {tableName} ({string.Join(",", selectedColumns)}) VALUES ";

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(';');

                // Create an array of selected values based on column indices
                string[] selectedValues = selectedColumns.Select(column => values[columnIndexMapping[column]]).ToArray();

                // Wrap string values with single quotes
                selectedValues = selectedValues.Select(value => $"'{value}'").ToArray();

                // Concatenate values to the insert SQL statement
                insertSql += $"({string.Join(", ", selectedValues)}),";
            }

            // Remove the trailing comma
            insertSql = insertSql.TrimEnd(',');

            connector.ExecuteNonQuery(insertSql);

        }



        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return csvData;
        }



    }
}
class MyClass
{
    public string indicator_name { get; set; }
    public string indicator_code { get; set; }

}