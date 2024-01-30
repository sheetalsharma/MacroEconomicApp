
using System.Data.SQLite;
using System.Data;

namespace MacroEconomicApp
{
    internal class SQLiteConnector
    {
        private readonly string connectionString;

        public SQLiteConnector(string databaseName)
        {
            connectionString = $"Data Source=C:\\Users\\ashni\\OneDrive\\Desktop\\MCB\\script\\MacroEconomicAnalysisDB.db;Version=3;";
        }

        public void ExecuteNonQuery(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        public List<string> GetTableNames()
        {
            List<string> tableNames = new List<string>();
 
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT name FROM sqlite_master WHERE type='table'";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader["name"].ToString());
                        }
                    }
                }
            }

            return tableNames;
        }


        public DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }
    }
}

