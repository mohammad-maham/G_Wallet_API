using System.Data;
using System.Reflection;

namespace OnSiteApii._Services
{
    public static class DataTableExtention
    {
        public static IEnumerable<T> AsEnumerable<T>(this DataTable dataTable) where T : new()
        {
            try
            {

                var dataList = new List<T>();
                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
                var objFieldNames = (from PropertyInfo aProp in typeof(T).GetProperties(flags)
                                     where aProp.GetCustomAttributes(typeof(DbColumnAttribute), false).Length > 0
                                     orderby aProp.Name ascending
                                     select new
                                     {
                                         Name = ((DbColumnAttribute)aProp.GetCustomAttributes(typeof(DbColumnAttribute), false)[0]).Name,
                                         // Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                                     }).ToList();

                var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                         orderby aHeader.ColumnName ascending
                                         select new
                                         {
                                             Name = aHeader.ColumnName,
                                             //Type = aHeader.DataType
                                         }).ToList();
                var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

                var fro = (from PropertyInfo aProp in typeof(T).GetProperties(flags)
                           where aProp.GetCustomAttributes(typeof(DbColumnAttribute), false).Length > 0
                           select aProp).ToList();

                foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
                {
                    var aTSource = new T();
                    foreach (var aField in commonFields)
                    {
                        PropertyInfo pi = null;
                        foreach (var _  in fro)
                        {
                            var dbColumn = (DbColumnAttribute)aTSource.GetType().GetProperty(_.Name)
                                .GetCustomAttributes(typeof(DbColumnAttribute)).FirstOrDefault();
                             if (aField.Name == dbColumn.Name)
                            {
                                pi = _;
                                break;
                            }
                        }

                        if (dataRow[aField.Name] != DBNull.Value)
                        {
                            var propertyType = pi.PropertyType;

                            if (pi.PropertyType.IsGenericType &&
                                pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                propertyType = Nullable.GetUnderlyingType(pi.PropertyType);

                            switch (Type.GetTypeCode(propertyType))
                            {

                                case TypeCode.Byte:
                                    pi.SetValue(aTSource, dataRow[aField.Name].ToString().ToByte(), null);
                                    break;
                                case TypeCode.Int16:
                                    pi.SetValue(aTSource, dataRow[aField.Name].ToString().ToInt(), null);
                                    break;
                                case TypeCode.Int32:
                                    pi.SetValue(aTSource, dataRow[aField.Name].ToString().ToInt(), null);
                                    break;
                                case TypeCode.Int64:
                                    pi.SetValue(aTSource, dataRow[aField.Name].ToString().ToLong(), null);
                                    break;
                                case TypeCode.Boolean:
                                    pi.SetValue(aTSource,  Convert.ToBoolean(dataRow[aField.Name]), null);
                                    break;
                                default:
                                    pi.SetValue(aTSource, dataRow[aField.Name].ToString(), null);
                                    break;
                            }
                        }
                        else
                            pi.SetValue(aTSource, null, null);
                    }
                    dataList.Add(aTSource);
                }
                return dataList;
            }
            catch (Exception ex)
            {
                Logger.Log(null, "TableConvertor Exception", ex.InnerException.ToString());
                throw ex;
            }
        }

        public static T AsOne<T>(this DataTable dataTable) where T : new()
        {
            try
            {
                var dataList = dataTable.AsEnumerable<T>();
                return dataList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Log(null, "TableConvertor Exception", ex.InnerException.ToString());
                throw ex;
            }
        }

        public static byte? ToByte(this string s)
        {
            byte i;
            if (byte.TryParse(s, out i)) return i;
            return null;
        }

        public static int? ToInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }
        public static long? ToLong(this string s)
        {
            long i;
            if (long.TryParse(s, out i)) return i;
            return null;
        }

        //  public class CommonFields
        //  {
        //    public  string Name { get; }
        //      public Type Type { get;  }
        //  }
        //public  class CommonFieldsCompare : IEqualityComparer<CommonFields>
        //  {

        //      public bool Equals(CommonFields x, CommonFields y)
        //      {
        //          return x.Name.Equals(y.Name);
        //      }

        //      public int GetHashCode(CommonFields x) => x.Name.GetHashCode()  ;

        //  }
    }

}

