using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SQLCode
{
    /// <summary>
    /// Contains, creates annd runs stored procedures for the database
    /// </summary>
    class StoredProcedures
    { //https://www.w3schools.com/sql/sql_stored_procedures.asp
        /// <summary>
        /// Creates all stored procedures in the database that the program will need.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="System.Data.SqlClient.SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="TargetParameterCountException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static void CreateAllStoredProcedures()
        {
            StorageSystemCore.Reporter.Log("Starting creation of stored procedures");
            MethodInfo[] methods = Type.GetType(typeof(StoredProcedures).ToString()).GetMethods(BindingFlags.NonPublic | BindingFlags.Static); //the binding flags ensures only static non-public methods are found.
            foreach(MethodInfo method in methods)
            {
                try
                {
                    string sql = (string)method.Invoke(null, null); //unsafe, if a method does not have a return type that can be casted to a string or void return type, the program will crash.
                    StorageSystemCore.Reporter.Log($"Trying to run: {sql}");
                    SQLControl.RunCommand(sql); //first null is given since it is not invoked in an instance and the second null is because there is no parameters in the functions
                    StorageSystemCore.Reporter.Log("Succeded in running.");
                }
                catch (Exception e) 
                {
                    StorageSystemCore.Reporter.Log("Failed in running.");
                    throw e;
                }
            }
            StorageSystemCore.Reporter.Log("Creation of stored procedures finalised");
        }

        private static string CreateFullView()
        {
            return
                "CREATE PROCEDURE SelectAllData " +
                    "AS " +
                    "SELECT * FROM Inventory;";
        }

        private static string CreatePartlyView()
        {
            return
                "CREATE PROCEDURE SelectPartlyData @Columns nvarchar(512) " +
                    "AS " +
                    "SELECT @Columns FROM Inventory;";
        }

        private static string CreateInsertWaresBasic() 
        {
            return
                "CREATE PROCEDURE InsertWareBasic @ID nvarchar(16), @Name nvarchar(128), @Amount int, @Type nvarchar(64) " +
                    "AS " +
                    "INSERT INTO Inventory (id,name,amount,type) " +
                    "Values (@ID,@Name,@Amount,@Type);";
        }

        private static string CreateInsertWareFull()
        {
            return
                "CREATE PROCEDURE InsertWareFull " +
                    "@ID nvarchar(16), @Name nvarchar(128), @Amount int, @Type nvarchar(64), " +
                    "@Information nvarchar(2048) null, @DangerCategory int null, @FlashPoint float null, @MinTemp float null, @BoilingPoint float null, @Volatile bit null " +
                    "AS " +
                    "INSERT INTO Inventory (id,name,amount,type,information,dangerCategory,flashPoint,minTemp,boilingPoint,volatile) " +
                    "Values (@ID,@Name,@Amount,@Type,@Information,@DangerCategory,@FlashPoint,@MinTemp,@BoilingPoint,@Volatile);";
        }

        private static string CreateDeleteWare()
        {
            return
                "CREATE PROCEDURE DeleteWare @WareToDelete nvarchar(16) " +
                    "AS " +
                    "DELETE FROM Inventory WHERE id = @WareToDelete;";
        }

        private static string CreateFindWareType()
        {
            return
                "CREATE PROCEDURE FindWareType @WareID nvarchar(16) " +
                    "As " +
                    "SELECT type FROM Inventory WHERE id = @WareID;";
        }

        private static string CreateSelectOneWareValues()
        {
            return
                "CREATE PROCEDURE SelectValuesFromOneWare @WareID nvarchar(16) " +
                    "AS " +
                    "SELECT * FROM Inventory WHERE id = @WareID;";
        }

        private static string CreateSelectPartValuesOfOneWare()
        {
            return
                "CREATE PROCEDURE SelectPartValuesFromOneWare @WareID nvarchar(16), @Columns nvarchar(512) " +
                    "AS " +
                    "SELECT @Columns FROM Inventory WHERE id = @WareID;";
        }

        private static string CreateUpdateWare()
        {
            return
                "CREATE PROCEDURE UpdateWare @WareID nvarchar(16), @Column nvarchar(512), @NewValue nvarchar(2048) " +
                    "AS " +
                    "UPDATE Inventory " +
                    "SET @Column = @NewValue " +
                    "WHERE id = @WareID;";
        }

        private static string CreateAddAmountWare()
        {
            return
                "CREATE PROCEDURE AddToWareAmount @WareID nvarchar(16), @Value int " +
                    "AS " +
                    "UPDATE Inventory " +
                    "SET amount = amount + @Value " +
                    "WHERE id = @WareID;";
        }

        private static string CreateRemoveAmountWare()
        {
            return
                "CREATE PROCEDURE RemoveFromWareAmount @WareID nvarchar(16), @Value int " +
                    "AS " +
                    "UPDATE Inventory " +
                    "SET amount = amount - @Value " +
                    "WHERE id = @WareID;";
        }

        private static string CreateGetColumnNames()
        {
            return
                "CREATE PROCEDURE GetColumnNames @Table nvarchar(24) " +
                    "AS " +
                    "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS " +
                    "WHERE TABLE_NAME=@Table;";
        }

        private static string CreateGetColumnNamesAndTypes()
        {
            return
                "CREATE PROCEDURE GetColumnNamesAndTypes " +
                    "AS " +
                    "SELECT DATA_TYPE, COLUMN_NAME " +
                    "FROM INFORMATION_SCHEMA.COLUMNS " +
                    "WHERE TABLE_NAME = 'Inventory'";
        }

        /// <summary>
        /// Not Working as MSSQL thinks @Column is the column name. Uses the update stored procedure to update the ware <paramref name="id"/>'s <paramref name="column"/> with the new value of <paramref name="newValue"/>.
        /// </summary>
        /// <param name="id">Ware ID to update</param>
        /// <param name="column">The Column to update</param>
        /// <param name="newValue">The new value</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void UpdateWareSP(string id, string column, string newValue)
        {
            if (id == null || column == null || newValue == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC UpdateWare @WareID = {id}, @Column = {column}, @NewValue = {newValue}";
            try { 
                SQLControl.RunCommand(sqlString);
            }
            catch(Exception e)
            {
                StorageSystemCore.Reporter.Report(e);
                Console.WriteLine($"Could not update: {e.Message}");
                StorageSystemCore.Support.WaitOnKeyInput();
            }
        }

        /// <summary>
        /// Uses the get type storage procedure to retrive the type of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ware id to retrive the type of</param>
        /// <returns>Returns the type of <paramref name="id"/> in a string list.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetTypeSP(string id)
        {
            if (id == null)
                throw new NullReferenceException();

            string sqlString =
                $"EXEC FindWareType @WareID = {id};";
            try
            {
                return SQLControl.GetValuesSingleWare(sqlString);
            }
            catch (Exception e)
            {
                throw e;
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not find the type: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                //return null;
            }
        }

        /// <summary>
        /// Uses the Select all data stored procedure to collect all ware information in the database.
        /// </summary>
        /// <returns>Returns all information of all wares in the database as a list of lists of strings.</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<List<string>> GetAllInformation()
        {
            string sqlString =
                "EXEC SelectAllData;";
            try { 
                return SQLControl.GetValuesMultiWares(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not get information: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                //return null;
                throw e;
            }
        }

        /// <summary>
        /// Not Working as MSSQL thinks @Column is the column name. Uses the Select partly data procedure to collect the data in <paramref name="columns"/> of all wares in the database.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns>Returns information from <paramref name="columns"/> of all wares in the database as a list of lists of strings.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<List<string>> GetPartlyInformation(string columns)
        {
            if(columns == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC SelectPartlyData @Columns = {columns};";
            try
            {
                return SQLControl.GetValuesMultiWares(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not get information: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                //return null;
                throw e;
            }
        }

        /// <summary>
        /// Uses the select values from one ware stored procedure to retrive all information of the ware with <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the ware to collect information from</param>
        /// <returns>Returns a list of strings, each string containg a column of data.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetInformationFromOneWareSP(string id)
        {
            if(id == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC SelectValuesFromOneWare @WareID = {id};";
            try { 
                return SQLControl.GetValuesSingleWare(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not get information: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                //return null;
                throw e;
            }
        }

        /// <summary>
        /// Not Working as MSSQL thinks @Column is the column name. Uses the select part values from one ware stored procedure to retive the information(s) of <paramref name="columns"/> of the ware with <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the ware to collect information from. </param>
        /// <param name="columns">The column(s) to get information(s) from. </param>
        /// <returns>Returns a list of strings, each string containg a column of data.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetPartlyInformationFromOneWareSP(string id, string columns)
        {
            if(id == null || columns == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC SelectPartValuesFromOneWare @WareID = {id}, @Columns = {columns};";
            try { 
                return SQLControl.GetValuesSingleWare(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not get information: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                //return null;
                throw e;
            }
        }

        /// <summary>
        /// Uses the delete ware stored procedure to delete the ware with the <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the ware to delete.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void RunDeleteWareSP(string id)
        {
            if(id == null)
                throw new NullReferenceException();
            string sqslString =
                $"EXEC DeleteWare @WareToDelete = {id};";
            try
            {
                SQLControl.RunCommand(sqslString);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Adds <paramref name="amount"/> to the total amount of the ware with the ID <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount to add</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void AddToWareAmountSP(string id, int amount)
        {
            if(id == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC AddToWareAmount @WareID = {id}, @Value = {amount}";
            try
            {
                SQLControl.RunCommand(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not add to ware: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                throw e;
            }
        }

        /// <summary>
        /// Removes <paramref name="amount"/> from the total amount of the ware with the ID <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount to remove</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void RemoveFromWareAmountSP(string id, int amount)
        {
            if(id == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC RemoveFromWareAmount @WareID = {id}, @Value = {amount}";
            try
            {
                SQLControl.RunCommand(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not add to ware: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                throw e;
            }
        }

        /// <summary>
        /// Uses the insert ware stored procedure to insert a new ware, into the database, with the basic column requirements filled out.
        /// </summary>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="name">The name of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="type">The type of the ware.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void InsertWareSP(string id, string name, int amount, string type)
        {
            if(id == null || name == null || type == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC InsertWareBasic @ID = {id}, @Name = {name}, @Amount = {amount}, @Type = {type}";
            try { 
            SQLControl.RunCommand(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not insert: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                throw e;
            }
        }

        /// <summary>
        /// Uses the insert ware stored procedure to insert a new ware, into the database, with the ability to set any column. 
        /// However, <paramref name="id"/>, <paramref name="amount"/>, <paramref name="name"/> and <paramref name="type"/> is needed.
        /// </summary>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="name">The name of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="type">The type of the ware.</param>
        /// <param name="information">Information about the ware.</param>
        /// <param name="dangerCategory">The danger category of the ware.</param>
        /// <param name="flashPoint">The flash point of the ware.</param>
        /// <param name="minTemp">The minimum temperature of the ware.</param>
        /// <param name="boilingPoint">The boiling point of the ware.</param>
        /// <param name="volatile">Whether the ware is volatile or not.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void InsertWareSP(string id, string name, int amount, string type, string information, string dangerCategory, string flashPoint, string minTemp, string boilingPoint, string @volatile)
        {
            string sqlString =
                $"EXEC InsertWareFull @ID = {id}, @Name = {name}, @Amount = {amount}, @Type = {type}, " +
                    $"@Information = {information ?? "null"}, @DangerCategory = {dangerCategory ?? "null"}, @FlashPoint = {flashPoint ?? "null"}, @MinTemp = {minTemp ?? "null"}, @BoilingPoint = {boilingPoint ?? "null"}, @Volatile = {@volatile ?? "null"}";
            try
            {
                SQLControl.RunCommand(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not insert: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static List<string> GetColumnNamesSP(string table)
        {
            if(table == null)
                throw new NullReferenceException();
            string sqlString =
                $"EXEC GetColumnNames @Table = {table}";
            try
            {
                return SQLControl.GetColumnNames(sqlString);
            }
            catch (Exception e)
            {
                //StorageSystemCore.Reporter.Report(e);
                //Console.WriteLine($"Could not retrive columns: {e.Message}");
                //StorageSystemCore.Support.WaitOnKeyInput();
                //return null;
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static string[] GetColumnNamesAndTypesSP(out string[] types)
        {
            string sqlString =
                $"EXEC GetColumnNamesAndTypes;";
            try
            {
                return SQLControl.GetColumnNamesAndTypes(sqlString, out types);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
