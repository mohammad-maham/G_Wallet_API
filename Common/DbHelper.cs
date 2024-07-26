using Microsoft.EntityFrameworkCore;

namespace OnSiteApii._Services
{
    public class DbHelper
    {
        private string connectionString { get; }
        //public static long GetPostgreSQLSequenceNextVal(DbContext db, string sequence)
        //{
        //    db.Database.OpenConnection();
        //    try
        //    {
        //        using System.Data.Common.DbCommand cmd = db.Database.GetDbConnection().CreateCommand();
        //        cmd.CommandText = @$"select nextval('{sequence}')";
        //        return (long)cmd.ExecuteScalar()!;
        //    }
        //    finally
        //    {
        //        db.Database.CloseConnection();
        //    }
        //}
    }
}