using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

public class PostgresDbHelper
{
    private readonly string _connectionString = ConfigurationManager.AppSetting["ConnectionStrings:GWalletDbContext"]!;

    public PostgresDbHelper()
    {
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }


    public DataTable RunQuery(string query)
    {
        using (var connection = GetConnection())
        {
            connection.OpenAsync();
            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    //Task.Run(() => adapter.Fill(dt));
                    return dt;
                }
            }
        }
    }
}