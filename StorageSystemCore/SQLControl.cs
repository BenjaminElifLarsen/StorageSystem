using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SQLCode
{
    class SQLControl
    {
        /// <summary>
        /// Name of the database.
        /// </summary>
        private static string database;
        /// <summary>
        /// The SQLConnection to a database.
        /// </summary>
        private static SqlConnection sqlConnection;

        /// <summary>
        /// Gets and sets the SQL Connection to a specific database.
        /// </summary>
        /// <value>Contains the sql connection used to connect with the database.</value>
        public static SqlConnection SQLConnection { get => sqlConnection; set => sqlConnection = value; }

        /// <summary>
        /// Gets and sets the name of the database.
        /// </summary>
        /// <value>Contains the name of the database that is in use.</value>
        public static string DataBase { get => database; set => database = value; }

        /// <summary>
        /// Gets and sets whether a database is in use or not.
        /// </summary>
        /// <value>Is true if a database is in use, else false.</value>
        public static bool DatabaseInUse { get; set; }

        /// <summary>
        /// Sanitises <paramref name="queryToSanitise"/> and returns it. 
        /// </summary>
        /// <param name="queryToSanitise"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string SanitiseSingleQuotes(string queryToSanitise)
        {
            if (queryToSanitise == null)
                throw new NullReferenceException();
            List<char> sanitised = new List<char>();
            foreach (char chr in queryToSanitise)
            {
                sanitised.Add(chr);
                if (chr == '\'')
                {
                    sanitised.Add('\'');
                }
            }

            return new string(sanitised.ToArray());
        }

        /// <summary>
        /// Creates a trusted connection string and returns it.
        /// </summary>
        /// <returns>Returns the trusted connection string.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string CreateConnectionString(string server, string database)
        {
            if (server == null || database == null)
                throw new NullReferenceException();
            SqlConnectionStringBuilder sqlCnt = new SqlConnectionStringBuilder();
            sqlCnt["Server"] = server;
            sqlCnt.InitialCatalog = database;
            sqlCnt.IntegratedSecurity = true;
            return sqlCnt.ToString();
        }

        /// <summary>
        /// Creates a connection string and returns it.
        /// </summary>
        /// <returns>Returns the connection string. </returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string CreateConnectionString(string server, string username, string password, string database)
        {
            if (server == null || username == null || password == null || database == null)
                throw new NullReferenceException();
            SqlConnectionStringBuilder sqlCnt = new SqlConnectionStringBuilder();
            sqlCnt["Server"] = server;
            sqlCnt["User Id"] = username;
            sqlCnt["Password"] = password;
            sqlCnt.InitialCatalog = database;

            return sqlCnt.ToString();
        }

        /// <summary>
        /// Removes a ware from from the database.
        /// </summary>
        /// <param name="sqlRemove"></param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void RemoveWare(string table, string sqlRemove)
        {
            if(table == null || sqlRemove == null)
                throw new NullReferenceException();
            string sqlCommand = $"Use {database}; Delete From {table} Where {sqlRemove}";
            RunCommand(sqlCommand);
        }

        /// <summary>
        /// Runs a query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to run.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void RunCommand(string sqlQuery)
        {
            try { 
                SqlCommand command = new SqlCommand(sqlQuery, SQLConnection);
                sqlConnection.Open();
                command.ExecuteNonQuery();
            }
            catch(SqlException e)
            {
                throw e;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Modifies one or more ware(s).
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="obj">Each key is a sql column and each value is the new value.</param>
        /// <param name="whereCondition">The where condition. Note that the 'Where' is already given.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void ModifyWare(string table, Dictionary<string, object> obj, string whereCondition)
        {
            if(table == null || obj == null || whereCondition == null)
                throw new NullReferenceException();
            string[] sqlColumns = new string[obj.Count];
            string[] sqlValues = new string[obj.Count];
            int pos = 0;
            foreach (KeyValuePair<string, object> entry in obj) //function this and the one in AddWare(string,dictionary<string,object>)
            {
                sqlColumns[pos] = entry.Key;
                sqlValues[pos] = entry.Value.ToString();
                pos++;
            }
            try { 
                ModifyWare(table, sqlColumns, sqlValues, whereCondition);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Modifies one or more ware(s).
        /// </summary>
        /// <param name="table">The sql table.</param>
        /// <param name="columnsToUpdate">The column(s) to modify in.</param>
        /// <param name="valuesToUpdateToo">The new value(s).</param>
        /// <param name="whereCondition">The where condition. Note that the 'Where' is already given.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static void ModifyWare(string table, string[] columnsToUpdate, string[] valuesToUpdateToo, string whereCondition)
        {
            if (table == null || columnsToUpdate == null || valuesToUpdateToo == null || whereCondition == null)
                throw new NullReferenceException();
            if (columnsToUpdate.Length != valuesToUpdateToo.Length)
                throw new IndexOutOfRangeException();
            string sqlCommand = $"Use {database}; Update {table} Set "; 
            for(int n = 0; n <columnsToUpdate.Length; n++)
            {
                sqlCommand += $"{columnsToUpdate[n]} = {valuesToUpdateToo[n]}";
                if (n != columnsToUpdate.Length - 1)
                    sqlCommand += ",";
            }
            sqlCommand += $"Where {whereCondition}";
            try
            {
                RunCommand(sqlCommand);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Tries to establish a connection to the database in <paramref name="connectionString"/>. Returns the connection if it could connect, else null.
        /// </summary>
        /// <param name="connectionString">The sql datbase connection string</param>
        /// <returns>Returns the sql connection</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static SqlConnection CreateConnection(string connectionString)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                connection.Close();
                SQLConnection = connection;
                    return connection;
                
            }
            catch (Exception e)
            {
                sqlConnection = null;
                throw e;
            }
        }

        /// <summary>
        /// Tries to establish a connection to the database out from the data in <paramref name="sqlInfo"/> and <paramref name="window"/>. Returns the connection if it could connect, else null.
        /// </summary>
        /// <param name="sqlInfo">Contains the information needed to create a database connection string</param>
        /// <param name="window">If true the connection will be using Window login, else SQL Server login</param>
        /// <returns>Returns true if it could establish a connection, else false.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool CreateConnection(string[] sqlInfo, bool window) 
        {
            try
            {
                string connect;
                if (window)
                    connect = CreateConnectionString(sqlInfo[0], sqlInfo[1]);
                else
                    connect = CreateConnectionString(sqlInfo[0], sqlInfo[1], sqlInfo[2], sqlInfo[3]);
                CreateConnection(connect);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Runs the <paramref name="query"/> and collects data entries for all wares and returns them. 
        /// </summary>
        /// <param name="query">The sql query to run.</param>
        /// <returns>Returns a list of string list. Each string list being a different ware and each string being a different data entry.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<List<string>> GetValuesMultiWares(string query)
        {
            try { 
                List<List<string>> information = new List<List<string>>();
                SqlCommand command = new SqlCommand(query, SQLConnection);
                SQLConnection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        information.Add(new List<string>());
                        int colAmount = reader.FieldCount;
                        for (int i = 0; i < colAmount; i++)
                            information[information.Count - 1].Add(reader[i].ToString());
                    }
                }
                SQLConnection.Close();
                return information;

            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gets the value(s) in <paramref name="sqlColumn"/> from the object with the specific <paramref name="ID"/>.
        /// </summary>
        /// <param name="sqlColumn">Contains the columns to collect data from.</param>
        /// <param name="ID">The ID of the ware to collect data from.</param>
        /// <returns>Returns a string list, each string being a different dataentry.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetValuesSingleWare(string[] sqlColumn, string ID)
        {
            if (sqlColumn == null || ID == null)
                throw new NullReferenceException();
            List<string> information = new List<string>();
            string query = $"Use {database}; Select ";
            for (int i = 0; i < sqlColumn.Length; i++)
            {
                query += sqlColumn[i];
                if (i != sqlColumn.Length - 1)
                    query += ", ";
            }
            query += $" From Inventory Where id = {ID};";
            try { 
                SqlCommand command = new SqlCommand(query, SQLConnection);
                SQLConnection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colAmount = reader.FieldCount;
                        for (int i = 0; i < colAmount; i++)
                            information.Add(reader[i].ToString());
                    }
                }
                SQLConnection.Close();
                return information;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Collects and returns the values, of the coloumn(s) given in <paramref name="sqlColumn"/>, of all wares in the database.
        /// </summary>
        /// <param name="sqlColumn">Contains the columns that should be collected data from.</param>
        /// <returns>Returns a list of string list. Each string list is a dataentry for a ware and each list is a different ware. </returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<List<string>> GetValuesAllWare(string[] sqlColumn)
        {
            if (sqlColumn == null)
                throw new NullReferenceException();
            try 
            { 
                List<List<string>> information = new List<List<string>>();
                string query = $"Use {database}; Select ";
                for (int i = 0; i < sqlColumn.Length; i++)
                {
                    query += sqlColumn[i];
                    if (i != sqlColumn.Length - 1)
                        query += ", ";
                }
                query += $" From Inventory;";
                SqlCommand command = new SqlCommand(query, SQLConnection);
                SQLConnection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        information.Add(new List<string>());
                        int colAmount = reader.FieldCount;
                        for (int i = 0; i < colAmount; i++)
                            information[information.Count-1].Add(reader[i].ToString());
                    }
                }
                SQLConnection.Close();
                return information;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Collects all column names.
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <returns>Returns a string list, each string being a column name.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetColumnNames(string query)
        {
            try
            {
                List<string> information = new List<string>();
                SqlCommand command = new SqlCommand(query, SQLConnection);
                SQLConnection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colAmount = reader.FieldCount;
                        for (int i = 0; i < colAmount; i++)
                            information.Add(reader[i].ToString());
                    }
                }
                SQLConnection.Close();
                return information;
            } 
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Runs <paramref name="query"/> in the sql database and returns a string list with the values.
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <returns>Returns a string list, each string being a different data entry.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetValuesSingleWare(string query)
        {
            try { 
                List<string> information = new List<string>(); 
                SQLConnection.Open();
                SqlCommand command = new SqlCommand(query, SQLConnection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colAmount = reader.FieldCount;
                        for (int i = 0; i < colAmount; i++)
                            information.Add(reader[i].ToString());
                    }
                }
                return information;
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        /// <summary>
        /// Collects all column names and datatypes using <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The sql query to run</param>
        /// <param name="types">The datatypes of the columns.</param>
        /// <returns>The sql column names.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static string[] GetColumnNamesAndTypes(string query, out string[] types)
        {
            try
            {
                List<string> columnsList = new List<string>();
                List<string> typesList = new List<string>();
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(query, SQLConnection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columnsList.Add(reader[1].ToString());
                        typesList.Add(reader[0].ToString());
                    }
                }
                types = typesList.ToArray();
                return columnsList.ToArray();
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Initialises the database creation.
        /// </summary>
        /// <param name="sqlInfo">Contains the information to build a connection string.</param>
        /// <param name="masterConnection">Contains the connection string to the master database.</param>
        /// <param name="window">Is true if using Window login Authentication.</param>
        /// <returns>Returns true if the database was created.</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static bool InitalitionOfDatabase(string[] sqlInfo, string masterConnection, bool window)
        {
            if (sqlInfo == null || masterConnection == null)
                throw new NullReferenceException();
            if ((window && sqlInfo.Length != 2) || (!window && sqlInfo.Length != 4))
                throw new IndexOutOfRangeException();
            try
            {
                StorageSystemCore.Reporter.Log($"Initalising database creation.");
                CreateConnection(masterConnection);

                //create database
                DatabaseCreation.InitialiseDatabase();

                CreateConnection(sqlInfo, window); //creates the main connection that is connected directly to the database.
                StoredProcedures.CreateAllStoredProcedures();
                DatabaseCreation.CreateDefaultEntries();
                StorageSystemCore.Reporter.Log("Database created.");
                return true;
            }
            catch (System.Data.SqlClient.SqlException e)
            { 
                throw e;
            }
        }

    }
}
