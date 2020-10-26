using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// Contains functions to modify ware amount, add wares and remove ware
    /// </summary>
    public class WareModifier
    {
        /// <summary>
        /// Adds <paramref name="amount"/> to the ware with the id <paramref name="ID"/>.
        /// </summary>
        /// <param name="ID">The id of the ware.</param>
        /// <param name="amount">The amount to add.</param>
        public static void AddToWare(string ID, int amount)
        {
            if (!SQLCode.SQLControl.DatabaseInUse)
                Publisher.PubWare.AddToWare(ID, amount);
            else
                SQLCode.StoredProcedures.AddToWareAmountSP($"'{ID}'",amount);
        }

        /// <summary>
        /// Removes <paramref name="amount"/> from the ware with the id <paramref name="ID"/>.
        /// </summary>
        /// <param name="ID">The id of the ware.</param>
        /// <param name="amount">The amount to remove.</param>
        public static void RemoveFromWare(string ID, int amount)
        {
            if (!SQLCode.SQLControl.DatabaseInUse)
                Publisher.PubWare.RemoveFromWare(ID, amount);
            else
                SQLCode.StoredProcedures.RemoveFromWareAmountSP($"'{ID}'", amount);
        }

        /// <summary>
        /// Removes a ware. Returns true if the item was found and removed else false
        /// </summary>
        /// <param name="ID">The ID of the ware to remove</param>
        /// <returns>Returns true if the item was found and removed else false</returns>
        public static bool RemoveWare(string ID)
        {
            if (Support.Confirmation()) {
                if (!SQLCode.SQLControl.DatabaseInUse)
                    return WareInformation.RemoveWare(ID);
                try { 
                    SQLCode.StoredProcedures.RunDeleteWareSP($"'{ID}'");
                }
                catch (Exception e)
                {
                    Support.ErrorHandling(e, $"Failed at removing ware: {e.Message}");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows for the modifcation of one or more values of the ware with <paramref name="ID"/>. 
        /// 
        /// </summary>
        /// <param name="ID"></param>
        public static void ModifyWare(string ID) 
        {
            if (!SQLCode.SQLControl.DatabaseInUse)
            {
                string[] options = null;
                try
                {
                    options = GenerateOptions(ID, SQLCode.SQLControl.DatabaseInUse);
                }
                catch (Exception e)
                {
                    Support.ErrorHandling(e, $"Encounted an error: {e.Message}");
                }
                if (options != null) { 
                    Dictionary<string, object> informations = WareInformation.GetWareInformation(ID, out List<Type> valueTypes);
                    byte? answer;
                    do
                    {
                        answer = VisualCalculator.MenuRun(options, "Select entry to modify"); 
                        if (answer != options.Length - 1) { 
                            object oldValue = informations[options[(byte)answer]];
                            if (valueTypes[(byte)answer].IsValueType)
                            {
                                try
                                {
                                    object newValue = CollectValue(valueTypes[(byte)answer], oldValue);
                                    Publisher.PubWare.AlterWare(ID, newValue, options[(byte)answer]);
                                }
                                catch (Exception e)
                                {
                                    ErrorHandling(e);
                                }
                            }
                            else
                            { //string and arrays goes here. 
                                if(valueTypes[(byte)answer].FullName == "System.String") {
                                    FillOutString(options, answer, oldValue);
                                }
                                else if (valueTypes[(byte)answer].BaseType.Name == "Array")
                                {
                                    FillOutArray(options, answer, valueTypes, oldValue);
                                }
                            }
                        }
                    } while (answer != options.Length - 1);
                }

            }
            else //SQL
            { //get the type of the ID, find the attributes of that ID
                string[] options = GenerateOptions(ID, SQLCode.SQLControl.DatabaseInUse); 
                string[] columns = new string[options.Length - 1];
                string[] allColumns = null;
                string[] allColumnTypes = null;
                try { 
                    allColumns = SQLCode.StoredProcedures.GetColumnNamesAndTypesSP(out allColumnTypes);
                }
                catch (Exception e)
                {
                    Support.ErrorHandling(e, $"Could not retrive column names and types: {e.Message}");
                }
                if(allColumns != null && allColumnTypes != null) 
                { 
                    for (int i = 0; i < columns.Length; i++)
                        columns[i] = options[i];
                    List<string> values = SQLCode.SQLControl.GetValuesSingleWare(columns, $"'{ID}'");
                    byte? answer;
                    do
                    {
                        answer = VisualCalculator.MenuRun(options, "Select entry to modify");
                        if(answer != options.Length - 1) { 
                            string oldValue = values[(byte)answer] != "" ? values[(byte)answer] : "Null";
                            OutPut.FullScreenClear();
                            OutPut.DisplayMessage($"Old Value was {oldValue}. Enter new Value: ",true);
                            string newValue = Console.ReadLine(); //MSSQL does not seem like it has arrays as a datatype
                            for(int i = 0; i < allColumns.Length; i++)
                            {
                                if (allColumns[i] == options[(byte)answer])
                                    if (allColumnTypes[i] == "nvarchar") 
                                    { 
                                        newValue = $"'{newValue}'";
                                        break;
                                    }
                            }
                            try 
                            { 
                                SQLCode.SQLControl.ModifyWare("Inventory", new string[] { options[(byte)answer] }, new string[] { newValue }, $"id = '{ID}'");
                            }
                            catch (Exception e)
                            {
                                Reporter.Report(e);
                                OutPut.FullScreenClear();
                                OutPut.DisplayMessage($"Could not create the ware: {e.Message}", true);
                                Support.WaitOnKeyInput();
                            }

                        }
                    } while (answer != options.Length - 1);
                }
            }

            void ErrorHandling(Exception e)
            {
                Reporter.Report(e);
                OutPut.FullScreenClear();
                OutPut.DisplayMessage(String.Format("Could not convert: {0}", e.InnerException.Message), true);
                Support.WaitOnKeyInput();
            }
            void FillOutString(string[] options, byte? answer, object oldValue)
            {
                OutPut.FullScreenClear();
                OutPut.DisplayMessage($"Old Value was {oldValue ?? "Null"}. Enter new Value: ", true);
                string newValue = Input.GetString();
                try 
                { 
                    Publisher.PubWare.AlterWare(ID, newValue, options[(byte)answer]);
                }
                catch(Exception e)
                {
                    Support.ErrorHandling(e, $"Encountered an error: {e.Message}");
                }

            }

            void FillOutArray(string[] options, byte? answer, List<Type> valueTypes, object oldValue)
            {
                List<object> objectList = new List<object>();
                string[] addValueOptions = new string[] { "Enter Value", "Done" };
                byte? valueAnswer;
                do
                {
                    valueAnswer = VisualCalculator.MenuRun(addValueOptions, "Add Data Entry");
                    if (valueAnswer == 0)
                    {
                        if (!valueTypes[(byte)answer].Name.Contains("String"))
                        { //non-string
                            try
                            {
                                objectList.Add(CollectValue(Type.GetType(valueTypes[(byte)answer].FullName.Remove(valueTypes[(byte)answer].FullName.Length - 2, 2)), oldValue)); //code inside of Type.GetType(...) converts an array type to a non-array type
                            }
                            catch (Exception e)
                            {
                                ErrorHandling(e);
                            }
                        }
                        else
                        { //string
                            OutPut.FullScreenClear();
                            objectList.Add(Input.GetString());
                        }
                    }
                } while (valueAnswer != addValueOptions.Length - 1);
                object[] objectArray = objectList.ToArray();
                try
                {
                    Publisher.PubWare.AlterWare(ID, objectArray, options[(byte)answer]);
                }
                catch (Exception e)
                {
                    Support.ErrorHandling(e, $"Encountered an erorr: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Used for unit testing 
        /// </summary>
        /// <param name="ID"></param>
        public static void RemoveWareTesting(string ID)
        {
            WareInformation.RemoveWare(ID);
        }

        /// <summary>
        /// Generates an array of selectable attributes belonging to the type of the ware with the <paramref name="ID"/>.
        /// Finally, it also add an "Exit" to the end of the array. 
        /// </summary>
        /// <param name="ID">The ID of the ware. </param>
        /// <param name="databseInUse">True if sql is used, false otherwise.</param>
        /// <returns></returns>
        private static string[] GenerateOptions(string ID, bool databseInUse)
        {
            Type type;
            byte n = databseInUse ? (byte)1 : (byte)0;
            if (databseInUse)
            {
                try
                {
                    type = Type.GetType("StorageSystemCore." + Support.RemoveSpace(SQLCode.StoredProcedures.GetTypeSP($"'{ID}'")[0]));
                } 
                catch (NullReferenceException e)
                {
                    throw e;
                }
            }
            else
                type = Publisher.PubWare.GetTypeFromWare(ID); 
            List<string[]> attributes = WareInformation.FindSearchableAttributes(type);
            string[] options = new string[attributes.Count + 1]; 
            for (int i = 0; i < options.Length - 1; i++) 
                options[i] = attributes[i][n];
            options[options.Length - 1] = "Done";
            return options;
        }

        /// <summary>
        /// Collects a value via Console.ReadLine and returns it as variable of <paramref name="type"/> wrapped in an object.
        /// </summary>
        /// <param name="type">The type the input should be converted too.</param>
        /// <param name="oldValue">The old value that will be displayed.</param>
        /// <returns></returns>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="TargetParameterCountException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private static object CollectValue(Type type, object oldValue)
        {
            Type support = typeof(Support);
            MethodInfo foundMethod = support.GetMethod("ConvertStringToVariableType", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo genericVersion = foundMethod.MakeGenericMethod(type);
            tryAgain:;
            try
            {
                OutPut.FullScreenClear();
                OutPut.DisplayMessage(String.Format($"Old Value was {oldValue ?? "Null"}. Enter new Value: "), true);
                string value = Input.GetString();
                object newValue = genericVersion.Invoke(null, new object[] {value});
                return newValue;
            }
            catch (Exception e)
            {
                if (e.InnerException.Message == "Input string was not in a correct format.")
                {
                    Reporter.Report(e);
                    OutPut.FullScreenClear();
                    OutPut.DisplayMessage("Could not convert. Reenter value.", true);
                    goto tryAgain;
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
