using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CommonLibrary.Helpers
{
    public class SqlDataBaseReader
    {
        private static SqlDataBaseReader _instance;

        private readonly string connectionString;

        public SqlDataBaseReader()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Fenergo"].ConnectionString;
        }

        public static SqlDataBaseReader INSTANCE
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SqlDataBaseReader();
                }

                return _instance;
            }
        }

        public DataTable ExecuteCommand(string stringCommand)
        {
            var result = new DataTable();
            var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                var command = new SqlCommand(stringCommand, connection);
                var dataReader = command.ExecuteReader();
                result.Load(dataReader);
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                System.Console.Write(ex.ToString());
            }

            return result;
        }
    }
}