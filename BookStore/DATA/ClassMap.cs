using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DATA
{
    public class ClassMap : ClassMapBase, IClassMap
    {
        private ILogger _logger = LogManager.GetLogger("ClassMap");

        public void Save<T>(T obj) where T : IEntity
        {
            SqlConnection sqlConnection = new SqlConnection(ConnString);
            SqlCommand sqlCom = sqlConnection.CreateCommand();

            Type classType = typeof(T);
            PropertyInfo[] props = properties(classType);
            string[] propNames = propertyNames(classType);
            Type[] propTypes = propertyTypes(classType);

            sqlCom.Parameters.AddWithValue("@" + propertyIdName(classType), propertyIdType(classType).ToSqlType());
            sqlCom.Parameters["@" + propertyIdName(classType)].Value = propertyId(classType).GetValue(obj);

            StringBuilder query = new StringBuilder("INSERT INTO " + tableName(classType) + " VALUES(@" + propertyIdName(classType) + ", ");

            for (int i = 0; i < propTypes.Count(); i++)
            {
                sqlCom.Parameters.AddWithValue("@" + propNames[i], propTypes[i].ToSqlType());
                sqlCom.Parameters["@" + propNames[i]].Value = props[i].GetValue(obj);
                query.Append("@" + propNames[i]);
                if (i == propTypes.Count() - 1)
                    query.Append(");");
                else
                    query.Append(", ");
            }

            sqlCom.CommandText = query.ToString();
            
            try
            {
                sqlConnection.Open();
                sqlCom.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                _logger.Error("Error in Save() execute", e);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        /// <summary>
        /// Record element in the table; if exists - overwrite
        /// </summary>
        /// <param name="Object"></param>
        public void Update<T>(T obj) where T : IEntity
        {
            if (!isDefineObject)
                Save(obj);
            else
            {
                SqlConnection sqlConnection = new SqlConnection(ConnString);
                SqlCommand sqlCom = sqlConnection.CreateCommand();
                Type classType = typeof(T);

                PropertyInfo[] props = properties(classType);
                string[] propNames = propertyNames(classType);
                Type[] propTypes = propertyTypes(classType);

                StringBuilder query = new StringBuilder("UPDATE " + tableName(classType) + " SET " + propertyIdName(classType)
                    + "=@" + propertyIdName(classType) + ", ");

                sqlCom.Parameters.AddWithValue("@" + propertyIdName(classType), propertyIdType(classType).ToSqlType());
                sqlCom.Parameters["@" + propertyIdName(classType)].Value = propertyId(classType).GetValue(obj);

                for (int i = 0; i < propTypes.Count(); i++)
                {
                    sqlCom.Parameters.AddWithValue("@" + propNames[i], propTypes[i]);
                    sqlCom.Parameters["@" + propNames[i]].Value = props[i].GetValue(obj);
                    query.Append(propNames[i] + "=@" + propNames[i]);
                    if (i != propTypes.Count() - 1)
                        query.Append(", ");
                }

                query.Append(" WHERE " + propertyIdName(classType) + "=@" + propertyIdName(classType) + ";");
                sqlCom.CommandText = query.ToString();
                try
                {
                    sqlConnection.Open();
                    sqlCom.ExecuteNonQuery();

                }
                catch (SqlException e)
                {
                    _logger.Error("Error in Update() execute", e);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void Delete<T>(T obj) where T : IEntity
        {
            if (!isDefineObject)
                return;
            else
            {
                SqlConnection sqlConnection = new SqlConnection(ConnString);
                SqlCommand sqlCom = sqlConnection.CreateCommand();
                Type classType = typeof(T);

                string query = "DELETE FROM " + tableName(classType) + " WHERE " +
                propertyIdName(classType) + "=@" + propertyIdName(classType) + ";";

                sqlCom.Parameters.AddWithValue("@" + propertyIdName(classType), propertyIdType(classType).ToSqlType());
                sqlCom.Parameters["@" + propertyIdName(classType)].Value = propertyId(classType).GetValue(obj);

                sqlCom.CommandText = query.ToString();

                try
                {
                    sqlConnection.Open();
                    sqlCom.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    _logger.Error("Error in Delete() execute", e);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        
        public IEnumerable<T> ReadWithCondition<T>(string conditionName, object conditionValue) where T : IEntity
        {
            SqlConnection sqlConnection = new SqlConnection(ConnString);
            SqlCommand sqlCom = sqlConnection.CreateCommand();
            Type classType = typeof(T);

            List<T> items = new List<T>();

            string query = "SELECT * FROM " + tableName(classType) + " WHERE " + conditionName + "=@" + conditionName + ";";

            sqlCom.Parameters.AddWithValue("@" + conditionName, (conditionValue.GetType()).ToSqlType());
            sqlCom.Parameters["@" + conditionName].Value = conditionValue;

            sqlCom.CommandText = query.ToString();

            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCom.ExecuteReader();

                int i = 0;
                while (sqlReader.Read())
                {
                    dynamic obj = Activator.CreateInstance(classType);
                    propertyId(classType).SetValue(obj, sqlReader[i]);
                    i++;
                    foreach (PropertyInfo propertyInfo in properties(classType))
                    {
                        propertyInfo.SetValue(obj, sqlReader[i]);
                        i++;
                    }
                    i = 0;
                    items.Add(obj);
                }

                sqlReader.Close();
            }
            catch (SqlException e)
            {
                _logger.Error("Error in Read() execute", e);
            }
            finally
            {
                sqlConnection.Close();
            }

            return items;
        }
        
        public IEnumerable<T> ReadAll<T>() where T : IEntity
        {
            Type classType = typeof(T);
            List<T> items = new List<T>();
            SqlConnection sqlConnection = new SqlConnection(ConnString);
            
            string query = "SELECT * FROM " + tableName(classType) + ";";
            SqlCommand sqlCom = new SqlCommand(query, sqlConnection);

            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCom.ExecuteReader();

                int i = 0;
                while (sqlReader.Read())
                {
                    dynamic obj = Activator.CreateInstance(classType);
                    propertyId(classType).SetValue(obj, sqlReader[i]);
                    i++;
                    foreach (PropertyInfo propertyInfo in properties(classType))
                    {
                        propertyInfo.SetValue(obj, sqlReader[i]);
                        i++;
                    }
                    i = 0;
                    items.Add(obj);
                }

                sqlReader.Close();
            }
            catch (SqlException e)
            {
                _logger.Error("Error in ReadAll() execute", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return items;
        }
    }
}
