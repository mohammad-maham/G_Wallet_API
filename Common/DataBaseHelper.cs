﻿using Microsoft.EntityFrameworkCore;

namespace G_Wallet_API.Common;

public class DataBaseHelper
{
    public static long GetPostgreSQLSequenceNextVal(DbContext db, string sequence)
    {
        db.Database.OpenConnection();
        try
        {
            using System.Data.Common.DbCommand cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @$"select nextval('{sequence}')";
            return (long)cmd.ExecuteScalar()!;
        }
        finally
        {
            db.Database.CloseConnection();
        }
    }
}
