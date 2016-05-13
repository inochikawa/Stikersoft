using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using DATA.Attributes;
using System.Text;
using NLog;

namespace DATA
{
    public class ClassMapBase
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        protected static string ConnString {
            get
            {
                return @"Data Source=DESKTOP-DTR60P5\SQL;Initial Catalog=BookDatabase;Integrated Security=SSPI;";
            }
        }
        
        public bool SetConnection
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection(ConnString);

                try
                {
                    sqlConnection.Open();
                    sqlConnection.Close();
                    return true;
                }
                catch (SqlException e)
                {
                    _logger.Error("Connection error", e);
                    return false;
                }
            }
        }


        /// <summary>
        /// Establishes a connection to the database and in case of success, if this table does not exist,
        /// creates a new table
        /// </summary>
        public void Initialize()
        {
            if (!SetConnection) return;
            if (existTable) return;

            Type classType = GetType();

            StringBuilder query = new StringBuilder("CREATE TABLE [dbo].[" + tableName(classType) + "] ([" + propertyIdName(classType) + "] " + propertyIdType(classType).ToSqlType() + " PRIMARY KEY, ");


            int colomnCount = propertyNames(classType).Count();
            for (int i = 0; i < colomnCount; i++)
            {
                query.Append("[" + propertyNames(classType)[i] + "] " + propertyTypes(classType)[i].ToSqlType());
                if (i == colomnCount - 1)
                    query.Append(")");
                else
                    query.Append(", ");
            }

            SqlConnection sqlConnection = new SqlConnection(ConnString);

            SqlCommand sqlCom = new SqlCommand(query.ToString(), sqlConnection);

            sqlConnection.Open();

            try
            {
                sqlCom.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                _logger.Error("Create table error", e);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private bool existTable
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection(ConnString);

                sqlConnection.Open();

                Type classType = GetType();

                if (Attribute.IsDefined(classType, typeof(Table)))
                {
                    string query = "SELECT COUNT(*) FROM " + tableName(classType) + ";";
                    SqlCommand sqlCom = new SqlCommand(query, sqlConnection);

                    try
                    {
                        sqlCom.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        _logger.Warn("Table absent", e);
                        sqlConnection.Close();
                        return false;
                    }
                }
                sqlConnection.Close();
                return true;
            }
        }

        /// <summary>
        /// It determines whether the object exists in the table
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        protected bool isDefineObject
        {
            get
            {
                Type classType = GetType();
                PropertyInfo propertyInfo = classType.GetProperties()[0];
                string query = "SELECT COUNT(*) FROM " + tableName(classType) + " WHERE Id LIKE '" + propertyInfo.GetValue(this) + "'";

                SqlConnection sqlConnection = new SqlConnection(ConnString);

                SqlCommand sqlCom = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                dynamic countObject = null;

                try
                {
                    SqlDataReader sqlReader = sqlCom.ExecuteReader();
                    sqlReader.Read();
                    countObject = sqlReader[0];
                    sqlReader.Close();
                    
                }
                catch (SqlException e)
                {
                    _logger.Warn("Object absent", e);
                    return false;
                }
                finally
                {
                    sqlConnection.Close();
                }

                if ((int)countObject == 0)
                    return false;
                else
                    return true;
            }
        }


        #region For work with table components --- Colomns name, type, table name...

        protected static PropertyInfo[] properties(Type Type)
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            PropertyInfo[] properies = Type.GetProperties();
            foreach (PropertyInfo property in properies)
                if (Attribute.IsDefined(property, typeof(Map)))
                    propertyInfos.Add(property);

            return propertyInfos.ToArray();
        }

        protected static PropertyInfo propertyId(Type Type)
        {
            PropertyInfo[] properies = Type.GetProperties();
            foreach (PropertyInfo property in properies)
                if (Attribute.IsDefined(property, typeof(Id)))
                    return property;
            return null;
        }

        /// <summary>
        /// It returns a string consisting of naming columns
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected static string columnNames(Type Type)
        {
            StringBuilder colomns = new StringBuilder("(");

            foreach (string colomnName in propertyNames(Type))
            {
                colomns.Append(colomnName);

                if (colomnName == propertyNames(Type)[propertyNames(Type).Count() - 1])
                    colomns.Append(")");
                else
                    colomns.Append(", ");
            }

            return colomns.ToString();
        }

        /// <summary>
        /// Returns the name of the table
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected static string tableName(Type Type)
        {

            StringBuilder tableName = new StringBuilder();

            var attributeValue = Attribute.GetCustomAttribute(Type, typeof(Table)) as Table;
            if (attributeValue.Name == null)
                tableName.Append(Type.Name);
            else
                tableName.Append(attributeValue.Name);

            return tableName.ToString();
        }

        /// <summary>
        /// Returns the column names for this type of class
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected static string[] propertyNames(Type Type)
        {
            PropertyInfo[] properies = Type.GetProperties();
            List<string> colomnNames = new List<string>();
            foreach (PropertyInfo property in properies)
            {
                if (Attribute.IsDefined(property, typeof(Map)))
                {
                    var attributeValue =
                        Attribute.GetCustomAttribute(property, typeof(Map)) as Map;

                    if (attributeValue.Name != null) colomnNames.Add(attributeValue.Name);
                    else colomnNames.Add(property.Name);
                }
            }

            return colomnNames.ToArray();
        }

        protected static string propertyIdName(Type Type)
        {
            PropertyInfo[] properies = Type.GetProperties();
            foreach (PropertyInfo property in properies)
            {
                if (Attribute.IsDefined(property, typeof(Id)))
                {
                    var attributeValue =
                        Attribute.GetCustomAttribute(property, typeof(Id)) as Id;

                    if (attributeValue.Name != null) return attributeValue.Name;
                    else return property.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the type of speakers for this type of class
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected static Type[] propertyTypes(Type Type)
        {
            PropertyInfo[] properies = Type.GetProperties();
            List<Type> types = new List<Type>();

            foreach (PropertyInfo property in properies)
                if (Attribute.IsDefined(property, typeof(Map)))
                    types.Add(property.PropertyType);

            return types.ToArray();
        }

        protected static Type propertyIdType(Type Type)
        {
            PropertyInfo[] properies = Type.GetProperties();

            foreach (PropertyInfo property in properies)
                if (Attribute.IsDefined(property, typeof(Id)))
                    return property.PropertyType;

            return null;
        }

        #endregion
    }
}
