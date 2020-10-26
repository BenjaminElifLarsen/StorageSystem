using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLCode
{
    /// <summary>
    /// Database creation class.
    /// </summary>
    static class DatabaseCreation
    {
        /// <summary>
        /// Creates the database
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private static void CreateDatabase()
        {
            if (SQLControl.DataBase == null)
                throw new NullReferenceException();
            try
            {
                SQLControl.RunCommand($"Use Master; CREATE DATABASE {SQLControl.DataBase}");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Creates the table and columns.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private static void CreateTableAndColumns()
        {
            if (SQLControl.DataBase == null)
                throw new NullReferenceException();
            string sqlString =
                $"Use {SQLControl.DataBase}; " +
                    "Create Table Inventory " +
                    "(id NVARCHAR(16) Not null, " +
                    "idValue INT Not null Identity(1,1), " +
                    "name NVARCHAR(40) Not null , " +
                    "amount INT Not null , " +
                    "type NVARCHAR(40) Not null, " +
                    "dangerCategory int null, " +
                    "flashPoint float null, " +
                    "minTemp float null," +
                    "boilingPoint float null, " +
                    "volatile bit null," +
                    "information NVARCHAR(2048) null, " +
                    "Primary Key(id, idValue) );";
            try 
            { 
                SQLControl.RunCommand(sqlString);
            } 
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Creates default wares. 
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void CreateDefaultEntries()
        {
            try
            {
                StoredProcedures.InsertWareSP("'ID-55t'", "'Water'", 25, "'Liquid'");
                StoredProcedures.InsertWareSP("'ID-123q'", "'Toaster'", 25, "'Electronic'");
                StoredProcedures.InsertWareSP("'MO.92z'", "'CiF3'", 1, "'Combustible Liquid'", "'Danger'", "4", null, null, null, null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Creates the databae, its tables and their columns.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void InitialiseDatabase()
        {
            try
            {
                CreateDatabase();
                CreateTableAndColumns();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
