namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var sqlConnection = new SqlConnection(@"Data Source=(localdb)\ProjectsV12;Initial Catalog=Database;Integrated Security=True;Pooling=False;Connect Timeout=30"))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("SELECT o.[Id], [Comment] FROM dbo.Comments o INNER JOIN @idsToRetrieve itr ON o.[Id] = itr.[Id]", sqlConnection))
                {
                    AddFilterTvp(sqlCommand, new[] { 2, 3 });

                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            Console.WriteLine("Retrieved Id {0} with comment \"{1}\"", sqlDataReader.GetInt32(0), sqlDataReader.GetString(1));
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        private static void AddFilterTvp(SqlCommand sqlCommand, IEnumerable<int> ids)
        {
            var dataTable = new DataTable();

            DataColumn idColumn = dataTable.Columns.Add("Id");

            foreach (int id in ids)
            {
                DataRow row = dataTable.NewRow();

                row[idColumn] = id;

                dataTable.Rows.Add(row);
            }

            SqlParameter tvpParam = sqlCommand.Parameters.AddWithValue("@idsToRetrieve", dataTable);

            tvpParam.SqlDbType = SqlDbType.Structured;
            tvpParam.TypeName = "dbo.FilterType";
        }
    }
}