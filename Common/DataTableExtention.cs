using System.Data;
using System.Reflection;

namespace G_Wallet_API.Common;

public static class DataTableExtention
{
    public static IEnumerable<T> AsEnumerable<T>(this DataTable dataTable) where T : new()
    {

        var properties = typeof(T).GetProperties()
                                  .Where(p => p.CanWrite);

        foreach (DataRow row in dataTable.Rows)
        {
            var item = new T();

            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    var value = row[property.Name];
                    if (value != DBNull.Value)
                    {
                        property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }

            yield return item;
        }
    }

    public static T AsOne<T>(this DataTable dataTable) where T : new()
    {

        return dataTable.AsEnumerable<T>().FirstOrDefault()!;

    }

}

