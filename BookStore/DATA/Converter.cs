using System;
using System.Collections.Generic;
using System.Linq;

namespace DATA
{
    public static class Converter
    {
        private static Dictionary<string, string> _dictionaryType = new Dictionary<string, string>();
        private static bool _createdDic = false;

        private static void setTypes()
        {
            if (!_createdDic)
            {
                _dictionaryType.Add("System.Int32", "INT");
                _dictionaryType.Add("System.Single", "FLOAT"); 
                _dictionaryType.Add("System.String", "TEXT");
                _dictionaryType.Add("System.Float", "FLOAT(20)");
                _dictionaryType.Add("System.Double", "FLOAT(20)");
                _dictionaryType.Add("System.Boolean", "BIT");
                _dictionaryType.Add("System.DateTime", "DATETIME"); 
                _dictionaryType.Add("System.Guid", "UNIQUEIDENTIFIER NOT NULL DEFAULT newid()");
                _dictionaryType.Add("System.Uri", "TEXT");
            }
            _createdDic = true;
        }

        public static string ToSqlType(this Type Type)
        {
            setTypes();
            try
            {
                return _dictionaryType[Type.FullName];
            }
            catch
            {
                return "NVARCHAR(50)";
            }
            
        }

        public static string ToDotNetType(this Type Type)
        {
            setTypes();
            try
            {
                return _dictionaryType.FirstOrDefault(x => x.Value == Type.ToString()).Key;
            }
            catch
            {
                return "System.String";
            }

        }
    }
}
