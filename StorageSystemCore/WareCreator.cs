using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    sealed public class WareCreator
    {
        /// <summary>
        /// Enum that contains binary flags used for missing fields regarding ware creation.
        /// </summary>
        [Flags]
        private enum Information : byte
        {
            Missing_ID = 0b_0000_0001, //1
            Missing_Name = 0b_0000_0010, //2
            Missing_Type = 0b_0000_0100, //4
            Missing_Amount = 0b_0000_1000 //8
        }

        /// <summary>
        /// Enum that contains binary flags used for checking validation regarding ID. Each flag is an invalid validation.
        /// </summary>
        [Flags]
        private enum Validation : int
        {
            Invalid_Length = 0b_0000_0000_0000_0001, //1
            Invalid_Values = 0b_0000_0000_0000_0010, //2
            Invalid_Lowercase = 0b_0000_0000_0000_0100, //4
            Invalid_Uppercase = 0b_0000_0000_0000_1000, //8
            Invalid_Special = 0b_0000_0000_0001_0000, //16
            Invalid_ValidCharsOnly = 0b_0000_0000_0010_0000 //32
        }


        private readonly WarePublisher warePublisher;

        private WareCreator() { }
        public WareCreator(WarePublisher warePublisher)
        {
            warePublisher.RaiseCreateWareEvent += CreateWareEventHandler;
            warePublisher.RaiseRemoveWareCreatorEvent += RemoveFromSubscriptionEventHandler;
            this.warePublisher = warePublisher;
        }

        private void CreateWare()
        {

            string ID = null;
            string name = null;
            string type = null;
            int? amount = null;
            bool goBack = false;
            string title = "Ware Creation Menu";
            string[] options = new string[] { "Name", "ID", "Type", "Amount", "Finalise", "Back" }; 
            string[] displayOptions = Support.DeepCopy(options);
            do
            {
                byte answer = VisualCalculator.MenuRun(displayOptions, title);
                switch (answer)
                {
                    case 0:
                        name = EnterName();
                        displayOptions[0] = options[0];
                        displayOptions[0]  += ": " + name;
                        break;

                    case 1:
                        ID = CreateID();
                        displayOptions[1] = options[1];
                        displayOptions[1] += ": " + ID;
                        break;

                    case 2:
                        type = SelectType();
                        displayOptions[2] = options[2];
                        displayOptions[2] += ": " + type;
                        break;

                    case 3:
                        amount = EnterAmount();
                        displayOptions[3] = options[3];
                        displayOptions[3] += ": " + amount;
                        break;

                    case 4:
                        Creation(ref goBack,ID,name,type,amount);
                        break;

                    case 5:
                        goBack = Support.Confirmation();
                        break;
                }

            } while (!goBack);

        }

        private void Creation(ref bool goBack, string ID, string name, string type, int? amount)
        {
            byte missingValues;
            if (!MissingInformation(ID, name, type, amount, out missingValues)) 
            {
                goBack = Support.Confirmation();
                if (goBack)
                {
                    if (SQLCode.SQLControl.DatabaseInUse)
                    {
                        Dictionary<string, Type> propertyNamesAndTypes = FindSQLProperties(Type.GetType("StorageSystemCore." + Support.RemoveSpace(type)));
                        if (propertyNamesAndTypes.Count > 0)
                            if (VisualCalculator.MenuRun(new string[] { "Yes", "No" }, "Do you want to enter extra information?") == 0)
                            {
                                List<string> selectedOptions = new List<string>(); 
                                List<string> sqlOptions = propertyNamesAndTypes.Keys.ToList(); //find a better option for collection, at some point. 
                                sqlOptions.Add("Done");
                                byte selected;
                                do
                                {
                                    selected = VisualCalculator.MenuRun(sqlOptions.ToArray(), "Select information to add");
                                    if (selected != sqlOptions.Count - 1)
                                        if (!selectedOptions.Contains(sqlOptions[selected]))
                                            selectedOptions.Add(sqlOptions[selected]);
                                } while (selected != sqlOptions.Count - 1);
                                if (selectedOptions.Count > 0)
                                {
                                    Dictionary<string, object> columnsAndValues = ArquiringInformation(Type.GetType("StorageSystemCore." + Support.RemoveSpace(type)), selectedOptions, propertyNamesAndTypes);
                                    AddWare(name, ID, type, (int)amount, columnsAndValues);
                                }
                            }
                            else
                                AddWare(name, ID, type, (int)amount);
                    }
                    else 
                    {
                        //only if not using a database
                        object[] filledOutParameters = null;
                        type = Support.RemoveSpace(type);
                        if (DoesMultipleConstructorsExist(Type.GetType("StorageSystemCore." + type))) //Does multiple constructor exist?
                            if (AddMoreInformation())  //asks if the user wants to input more information
                            {
                                string[] extraParameters = CreateSelectableConstructorList(Type.GetType("StorageSystemCore." + type));
                                byte selectedCtor = SelectConstructor(extraParameters);
                                filledOutParameters = ArquiringInformation(Type.GetType("StorageSystemCore." + type), selectedCtor);

                            }
                        AddWare(name, ID, type, (int)amount, filledOutParameters);
                    }
                }
            }
            else //informs the user of missing values. 
            {
                PrintOutMissingInformation(missingValues);
                Support.WaitOnKeyInput();
            }
        }

        /// <summary>
        /// Prints out the missing information.
        /// </summary>
        /// <param name="missingValues">'Binary flag' that contains the information about missing ware information.</param>
        private void PrintOutMissingInformation(byte missingValues)
        {
            string baseMessage = "The following is missing: {0}";
            string missingInfo = "";
            if ((missingValues & (byte)Information.Missing_ID) == (byte)Information.Missing_ID)
                missingInfo += "ID ";
            if ((missingValues & (byte)Information.Missing_Name) == (byte)Information.Missing_Name)
                missingInfo += "Name ";
            if ((missingValues & (byte)Information.Missing_Type) == (byte)Information.Missing_Type)
                missingInfo += "Type ";
            if ((missingValues & (byte)Information.Missing_Amount) == (byte)Information.Missing_Amount)
                missingInfo += "Amount ";
            OutPut.FullScreenClear();
            OutPut.DisplayMessage(string.Format(baseMessage, missingInfo));
        }

        /// <summary>
        /// Asks the user to select a constructor from <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Returns the byte that belongs to the entry in <paramref name="options"/> that was selected. </returns>
        private byte SelectConstructor(string[] options)
        {
            //Console.Clear();
            return VisualCalculator.MenuRun(options, "Select more information");
        }

        /// <summary>
        /// Creates a string array, where each entry contains a string with all non-base variable names, and returns it. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns a string array. Each index consist of a string with the names of all parameters of a specific constructor.</returns>
        private string[] CreateSelectableConstructorList(Type type) 
        {
            List<List<string>> ctorsFromClass = WareInformation.FindExtraConstructorsParameterNames(type);
            List<string> baseCtorVariables = WareInformation.BasicConstructorVariableNames;
            List<string> tempCtors = new List<string>();
            string[] ctorArray;
            for (int n = 0; n < ctorsFromClass.Count; n++)
            {
                tempCtors.Add("");
                for (int m = 0; m < ctorsFromClass[n].Count; m++)
                {
                    if (!baseCtorVariables.Contains(ctorsFromClass[n][m]))
                        tempCtors[tempCtors.Count - 1] += ctorsFromClass[n][m] + " ";
                }
            }
            tempCtors.RemoveAll(IsEmpty);
            ctorArray = tempCtors.ToArray();
            return ctorArray;

            bool IsEmpty(string str)
            {
                return str == null || str == "";
            }
        }

        /// <summary>
        /// Asks the user to import the values of the constructor from WareInformation.GetConstructorParameterNamesAndTypes with index of <paramref name="number"/> with the type of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the ware whichs constructor(s)s should be found</param>
        /// <param name="number">The index of the selected constructor.</param>
        /// <returns>Returns an object array containing all the extra information that is needed for a specific constructor</returns>
        private object[] ArquiringInformation(Type type, byte number)
        {
            Dictionary<string, Type> parameters = WareInformation.GetConstructorParameterNamesAndTypes(type)[number]; //consider replacing number and type with Dictionary<string, Type> and let the caller pass WareInformation.GetConstructorParameterNamesAndTypes(type,null)[number] as an arugument 
            object[] parameterValues = new object[parameters.Count];
            string[] parameterNames = parameters.Keys.ToArray();
            Type parameterType;
            Support.ActiveCursor();
            for (int i = 0; i < parameterValues.Length; i++)
            {
                parameterType = parameters[parameterNames[i]];
                if (parameterType.IsValueType)
                {
                    Type support = typeof(Support);
                    MethodInfo foundMethod = support.GetMethod("ConvertStringToVariableType", BindingFlags.NonPublic | BindingFlags.Static);
                    MethodInfo genericVersion = foundMethod.MakeGenericMethod(parameterType);
                    tryAgain:;
                    try
                    {
                        OutPut.FullScreenClear();
                        OutPut.DisplayMessage(String.Format("Please Enter {0}", parameterNames[i]), true);
                        string value = Input.GetString();
                        parameterValues[i] = genericVersion.Invoke(null, new object[] { value });
                    }
                    catch (Exception e)
                    {
                        if(e.InnerException.Message == "Input string was not in a correct format."){
                            Reporter.Report(e);
                            OutPut.FullScreenClear();
                            OutPut.DisplayMessage("Could not convert. Reenter value.", true);
                            goto tryAgain;
                        }
                        else 
                        { 
                            Reporter.Report(e);
                            OutPut.FullScreenClear();
                            parameterValues[i] = Support.GetDefaultValueFromValueType(parameterType.Name); 
                            OutPut.DisplayMessage($"Could not convert. Value set to {parameterValues[i]}. Value can be modified using the Modify menu: {Environment.NewLine}" + e.InnerException.Message, true);
                            Support.WaitOnKeyInput();
                        }
                    }
                }
                else
                {
                    if(parameterType.FullName == "System.String") 
                    {
                        OutPut.FullScreenClear();
                        OutPut.DisplayMessage(String.Format("Please Enter {0}", parameterNames[i]), true);
                        parameterValues[i] = Input.GetString();
                    }
                    else if (parameterType.BaseType.Name == "Array")
                    {
                        List<object> objectList = new List<object>();
                        string[] addValueOptions = new string[] { "Enter Value", "Done" };
                        byte? valueAnswer;
                        do
                        {
                            valueAnswer = VisualCalculator.MenuRun(addValueOptions, "Add Data Entry");
                            if (valueAnswer == 0)
                            {
                                if (!parameterType.Name.Contains("String") )
                                { //non-string
                                    try //try catch if the makeGenericMethod fails.
                                    {
                                        tryAgain:;
                                        Type support = typeof(Support); 
                                        MethodInfo foundMethod = support.GetMethod("ConvertStringToVariableType", BindingFlags.NonPublic | BindingFlags.Static);
                                        MethodInfo genericVersion = foundMethod.MakeGenericMethod(Type.GetType(parameterType.FullName.Remove(parameterType.FullName.Length - 2)));
                                        try
                                        {
                                            OutPut.FullScreenClear(); 
                                            OutPut.DisplayMessage(String.Format("Please Enter {0}", parameterNames[i]), true);
                                            string value = Input.GetString();
                                            objectList.Add(genericVersion.Invoke(null, new object[] { value }));
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
                                                Reporter.Report(e);
                                                OutPut.FullScreenClear();
                                                OutPut.DisplayMessage(String.Format("Could not convert. Please reenter: {0}", e.InnerException.Message));
                                                Support.WaitOnKeyInput();
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Reporter.Report(e);
                                        OutPut.FullScreenClear();
                                        OutPut.DisplayMessage(string.Format("Could not convert: {0}", e.Message));
                                        Support.WaitOnKeyInput();
                                    }
                                }
                                else
                                { //string
                                    OutPut.FullScreenClear();
                                    OutPut.DisplayMessage("Enter Value: ", true);
                                    objectList.Add(Input.GetString());
                                }
                            }
                        } while (valueAnswer != addValueOptions.Length - 1);
                        if(objectList.Count > 0) { 
                            object[] objectArray = objectList.ToArray();
                            MethodInfo foundMethod = GetType().GetMethod("ArrayConversion", BindingFlags.Instance | BindingFlags.NonPublic);
                            MethodInfo genericVersion = foundMethod.MakeGenericMethod(Type.GetType(parameterType.FullName.Remove(parameterType.FullName.Length - 2)));
                            parameterValues[i] = genericVersion.Invoke(this,new object[] {objectArray });
                        }
                        else
                            parameterValues[i] = null;
                    }
                } 

            }
            Support.DeactiveCursor();
            return parameterValues;
        }

        /// <summary>
        /// Asks the user the enter the information about a ware of <paramref name="type"/>. The data names that is needed is <paramref name="sqlColumns"/> and the data types are in <paramref name="keyValuePairs"/>. //explain better
        /// </summary>
        /// <param name="type">The type of the ware.</param>
        /// <param name="sqlColumns">The sql columns to arquier data for.</param>
        /// <param name="keyValuePairs"><paramref name="sqlColumns"/> with their datatype. </param>
        private Dictionary<string,object> ArquiringInformation(Type type, List<string> sqlColumns, Dictionary<string,Type> keyValuePairs)
        {
            Support.ActiveCursor();
            Dictionary<string, object> nameAndValues = new Dictionary<string, object>();
            PropertyInfo[] propertyInfoes = type.GetProperties();
            foreach(PropertyInfo propertyInfo in propertyInfoes)
            {//write a Array.Contain(string) in Support at some point
                foreach (Attribute attre in propertyInfo.GetCustomAttributes())
                    if (attre is WareSeacheableAttribute info)
                    {
                        if (sqlColumns.Contains(info.SQLName))
                        {
                            if (keyValuePairs[info.SQLName].IsValueType) 
                            {
                                tryAgain:;
                                Type support = typeof(Support);
                                MethodInfo foundMethod = support.GetMethod("ConvertStringToVariableType", BindingFlags.NonPublic | BindingFlags.Static);
                                MethodInfo genericVersion = foundMethod.MakeGenericMethod(keyValuePairs[info.SQLName]);
                                try 
                                {
                                    OutPut.FullScreenClear();
                                    OutPut.DisplayMessage(String.Format("Please Enter {0}", info.Name), true);
                                    string value = Input.GetString();
                                    nameAndValues.Add(info.SQLName, genericVersion.Invoke(null, new object[] { value }));
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
                                        Reporter.Report(e);
                                        OutPut.FullScreenClear();
                                        OutPut.DisplayMessage(String.Format("Could not convert. Value set to 0: {0}", e.InnerException.Message));
                                        nameAndValues.Add(info.SQLName, Support.GetDefaultValueFromValueType(keyValuePairs[info.SQLName].Name.ToString())); //needs some more testing
                                        Support.WaitOnKeyInput();
                                    }
                                }
                            }
                            else
                            {
                                OutPut.FullScreenClear();
                                OutPut.DisplayMessage(String.Format("Please Enter {0}", info.Name),true);
                                string value = Input.GetString();
                                nameAndValues.Add(info.SQLName, value);
                            }
                        }
                    }
            }
            Support.DeactiveCursor();
            return nameAndValues;
        }


        /// <summary>
        /// Asks if the user wants to input more information or not. Returns true if they want too.
        /// </summary>
        /// <returns>Returns true if the user wants to input more information else false.</returns>
        private bool AddMoreInformation()
        {
            string title = "Do you want to add more information?";
            string[] options = new string[] {"Yes","No" };
            byte answer = VisualCalculator.MenuRun(options, title);
            return answer == 0;
        }

        /// <summary>
        /// Checks if <paramref name="type"/> contains more than one constructor. 
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Returns true if more than 1 constructor exist, else false.</returns>
        private bool DoesMultipleConstructorsExist(Type type)
        {
            return WareInformation.FindExtraConstructorsParameterNames(type).Count > 1;
        }

        /// <summary>
        /// Acts as the "select constructor, add name and type of its "parameters"" functions version for the sql database.
        /// </summary>
        /// <param name="type">The type to find sql properties of.</param>
        /// <returns>Returns a dictionary where each string is a sql column name and each value is the datatype of the column.</returns>
        private Dictionary<string,Type> FindSQLProperties(Type type) 
        { 
            Dictionary<string, Type> namesAndValues = new Dictionary<string, Type>();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
                    foreach (Attribute attre in propertyInfo.GetCustomAttributes())
                        if (attre is WareSeacheableAttribute info)
                        {
                            if (!WareInformation.BasicSQLNames.Contains(info.SQLName)) { 
                                Type propertyType = propertyInfo.GetMethod.ReturnType;
                                if (Nullable.GetUnderlyingType(propertyType) != null)
                                {
                                    string typeString = propertyType.FullName.Split(',')[0].Split("[[")[1]; //converts the nullable fullname to a string that can be used in Type.GetType(string).
                                    propertyType = Type.GetType(typeString);
                                }
                                namesAndValues.Add(info.SQLName, propertyType);
                        }
                    }
            return namesAndValues;
        }

        /// <summary>
        /// Checks if any of the parameters are null. Returns true if information is missing, else false.
        /// If any inforamtion is missing returns a combined binary value, <paramref name="missingValue"/>, that indicates which values are missing:
        /// </summary>
        /// <remarks>
        /// <para>0000_0001 = <paramref name="id"/> (1), </para>
        /// <para>0000_0010 = <paramref name="name"/> (2), </para>
        /// <para>0000_0100 = <paramref name="type"/> (4), </para>
        /// <para>0000_1000 = <paramref name="amount"/> (8). </para>
        /// </remarks>
        /// <param name="id">ID to check</param>
        /// <param name="name">Name to check</param>
        /// <param name="type">Type to check</param>
        /// <param name="amount">Amount to check</param>
        /// <param name="missingValue">A "binary flag" that indicates which values are missing.</param>
        /// <returns>Returns true if any information is missing, else false.</returns>
        private bool MissingInformation(string id, string name, string type, int? amount, out byte missingValue)
        {
            missingValue = 0b_0000_0000;
            if (id == null)
                missingValue = (byte)(missingValue ^ (byte)Information.Missing_ID);
            if (name == null)
                missingValue = (byte)(missingValue ^ (byte)Information.Missing_Name);
            if (type == null)
                missingValue = (byte)(missingValue ^ (byte)Information.Missing_Type);
            if (amount == null)
                missingValue = (byte)(missingValue ^ (byte)Information.Missing_Amount);
            if (missingValue == 0)
                return false;
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Collects and returns the name of the ware.
        /// </summary>
        /// <param name="message">The message to display for the user</param>
        /// <returns>Returns the name of the ware. </returns>
        private string EnterName(string message = "Enter Product Name")
        {
            return Support.CollectString(message);
        }

        /// <summary>
        /// Collects and returns the amount of the ware.
        /// </summary>
        /// <param name="message">The message to display for the user</param>
        /// <returns>Returns the amount of the ware.</returns>
        private int EnterAmount(string message = "Enter Amount")
        {
            return Support.CollectValue(message);
        }

        /// <summary>
        /// Allows the user to create a new ID while ensuring the ID is valid and unique. 
        /// </summary>
        /// <returns>Returns an unique ID for the ware. </returns>
        private string CreateID()
        {
            string ID_;
            OutPut.FullScreenClear();
            OutPut.DisplayMessage("Enter Valid Product ID", true);
            //Console.Clear();
            //Console.WriteLine("Enter Valid Product ID"); 
            Support.ActiveCursor();
            int binaryFlag;
            do
            {
                do
                {
                    ID_ = Input.GetString();//Console.ReadLine().Trim();
                    binaryFlag = ValidID(ID_);
                    if (binaryFlag != 0)
                        WriteOutIDErrors(binaryFlag);
                } while (binaryFlag != 0);
            } while (!UniqueID(ID_)); 
            Support.DeactiveCursor();
            return ID_;
        }

        /// <summary>
        /// Checks if an ID is unique or not. If not unique it will informs the user about it.
        /// </summary>
        /// <param name="IDToCheck">ID to check.</param>
        /// <returns>Returns true if the ID is unique else false.</returns>
        private bool UniqueID(string IDToCheck)
        {
            if (!Support.UniqueID(IDToCheck, SQLCode.SQLControl.DatabaseInUse))
            {
                OutPut.DisplayMessage("ID is not unique. Enter a new ID", true);//Console.WriteLine("ID is not unique. Enter a new ID");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if <paramref name="IDToCheck"/> is valid. If any parts of <paramref name="IDToCheck"/> it will add a value to the return result using binary.
        /// </summary>
        /// <remarks>
        /// <para>Length invalid: 0b_0000_0001 = 1.</para>
        /// <para>Number invalid: 0b_0000_0010 = 2.</para>
        /// <para>Lowercase invalid: 0b_0000_0100 = 4.</para>
        /// <para>Uppercase invalid: 0b_0000_1000 = 8.</para>
        /// <para>Special Chars invalid: 0b_0001_0000 = 16.</para>
        /// <para>Valid Chars invalid: 0b_0010_0000 = 32.</para>
        /// </remarks>
        /// <param name="IDToCheck">The ID to validate.</param>
        /// <returns>Returns a binary flag with a value indicating errors in <paramref name="IDToCheck"/>.</returns>
        private int ValidID(string IDToCheck) //it does not mater sense that this method writes out the error.
        { //have enums for this and the other one
            int errorFlag = 0b_0000_0000_0000_0000;
            if (!RegexControl.IsValidLength(IDToCheck))             
                errorFlag ^= (int)Validation.Invalid_Length;            
            if (!RegexControl.IsValidValues(IDToCheck))             
                errorFlag ^= (int)Validation.Invalid_Values;            
            if (!RegexControl.IsValidLettersLower(IDToCheck))             
                errorFlag ^= (int)Validation.Invalid_Lowercase;            
            if (!RegexControl.IsValidLettersUpper(IDToCheck))            
                errorFlag ^= (int)Validation.Invalid_Uppercase;            
            if(!RegexControl.IsValidSpecial(IDToCheck))             
                errorFlag ^= (int)Validation.Invalid_Special;          
            if(!RegexControl.IsValidCharsOnly(IDToCheck)) 
                errorFlag ^= (int)Validation.Invalid_ValidCharsOnly;
            return errorFlag;
        }

        private void WriteOutIDErrors(int errorFlag)
        {
            string baseMessage = "Invalid: {0}";
            string errors = "";
            StringBuilder stringBuilder = new StringBuilder(errors);
            if ((errorFlag & (int)Validation.Invalid_Length) == (int)Validation.Invalid_Length)
                stringBuilder.Append("Wrong Length, min = 6, max = 16. ");
            if ((errorFlag & (int)Validation.Invalid_Values) == (int)Validation.Invalid_Values)
                stringBuilder.Append("No numbers. ");
            if ((errorFlag & (int)Validation.Invalid_Lowercase) == (int)Validation.Invalid_Lowercase)
                stringBuilder.Append("No lowercase letters. ");
            if ((errorFlag & (int)Validation.Invalid_Uppercase) == (int)Validation.Invalid_Uppercase)
                stringBuilder.Append("No uppercase letters. ");
            if ((errorFlag & (int)Validation.Invalid_Special) == (int)Validation.Invalid_Special)
                stringBuilder.Append($"No special symbols: \"{RegexControl.GetSpecialSigns}\". ");
            if ((errorFlag & (int)Validation.Invalid_ValidCharsOnly) == (int)Validation.Invalid_ValidCharsOnly)
                stringBuilder.Append("Contains invalid symbols or letter. ");
            OutPut.DisplayMessage(String.Format(baseMessage, stringBuilder.ToString()),true);
            //Support.WaitOnKeyInput();
        }

        /// <summary>
        /// Allows for the selection of a ware type.
        /// </summary>
        /// <returns></returns>
        private string SelectType()
        {
            string[] possibleTypes = WareInformation.FindWareTypes().ToArray(); //should handle an empty list

            return possibleTypes[VisualCalculator.MenuRun(possibleTypes,"Select Type")];
        }

        /// <summary>
        /// Subscribes the a specific function, CreateWare(), of the class instance to an event for ware creation. 
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The parameters of the event</param>
        private void CreateWareEventHandler(object sender, ControlEvents.CreateWareEventArgs e) 
        {
            CreateWare();
        }

        /// <summary>
        /// Unsubscribes the class from the ware creation event.  
        /// </summary>
        /// <param name="warePublisher"></param>
        private void RemoveFromSubscriptionEventHandler(object sender, ControlEvents.RemoveWareCreatorEventArgs e)
        {

            warePublisher.RaiseCreateWareEvent -= CreateWareEventHandler;
            warePublisher.RaiseRemoveWareCreatorEvent -= RemoveFromSubscriptionEventHandler;
        }

        /// <summary>
        /// Converts an object[] <paramref name="arrayToConvert"/> to the datatype <typeparamref name="T"/>.
        /// Given how the method is written it is needed be called via reflection as a generic method
        /// </summary>
        /// <typeparam name="T">The type that should be converted to</typeparam>
        /// <param name="arrayToConvert">The array to convert</param>
        /// <returns>Returns a converted version of <paramref name="arrayToConvert"/>.</returns>
        private T[] ArrayConversion<T>(object[] arrayToConvert)
        {
            try { 
                T[] convertedArray = new T[arrayToConvert.Length];
                for (int i = 0; i < convertedArray.Length; i++)
                {
                    convertedArray[i] = (T)Convert.ChangeType(arrayToConvert[i], typeof(T));
                }
                return convertedArray;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Adds a new ware of <paramref name="type"/> with the basic values of <paramref name="name"/>, <paramref name="id"/> and <paramref name="amount"/>. 
        /// More arguments can be given in <paramref name="extra"/> in an order that fits a constructor of <paramref name="type"/>.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The id of the ware.</param>
        /// <param name="type">The type of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="extra">Extra information for the constructor of <paramref name="type"/>.</param>
        public void AddWare(string name, string id, string type, int amount, object[] extra)
        {
            type = Support.RemoveSpace(type);
            Type test = Type.GetType("StorageSystemCore." + type);
            int lengthToAdd = extra != null ? extra.Length : 0;
            object[] dataObject = new object[4 + lengthToAdd];
            dataObject[0] = name;
            dataObject[1] = id;
            dataObject[2] = amount;
            for (int i = 3; i < dataObject.Length - 1; i++)
                dataObject[i] = extra[i - 3];
            dataObject[dataObject.Length - 1] = Publisher.PubWare;
            try
            {
                WareInformation.Add((Ware)Activator.CreateInstance(test, dataObject));
            }
            catch (Exception e)
            {
                Support.ErrorHandling(e, $"Could not add ware: {e.Message}");
            }
        }

        /// <summary>
        /// Adds a new ware of <paramref name="type"/> with the basic values of <paramref name="name"/>, <paramref name="id"/> and <paramref name="amount"/>.
        /// Creates the ware in the database. 
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="type">The type of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        public void AddWare(string name, string id, string type, int amount) //move later to its final class, the WareCreator (after all, that is the calls that creates wares)
        {
            SQLCode.StoredProcedures.InsertWareSP($"'{id}'", $"'{name}'", amount, $"'{type}'");
        }

        /// <summary>
        /// Adds a new ware of <paramref name="type"/> with the basic values of <paramref name="name"/>, <paramref name="id"/> and <paramref name="amount"/>.
        /// Information stored in <paramref name="columnsAndValues"/> will be used to add extra information about the ware. 
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="type">The type of the ware.</param>
        /// <param name="amount">The amount of the ware. </param>
        /// <param name="columnsAndValues">Extra information to add to the ware. The keys are sql columns.</param>
        public void AddWare(string name, string id, string type, int amount, Dictionary<string, object> columnsAndValues)
        {
            object information;
            columnsAndValues.TryGetValue("information", out information);
            string informationString = $"'{information}'" == "" ? null : $"'{information}'";

            object dangerCategory;
            columnsAndValues.TryGetValue("dangerCategory", out dangerCategory);
            string dangerCategoryString = $"{dangerCategory}" == "" ? null : $"{dangerCategory}";

            object flashPoint;
            columnsAndValues.TryGetValue("flashPoint", out flashPoint);
            string flashPointString = $"{flashPoint}" == "" ? null : $"{flashPoint}";

            object minTemp;
            columnsAndValues.TryGetValue("minTemp", out minTemp);
            string minTempString = $"{minTemp}" == "" ? null : $"{minTemp}";

            object boilingPoint;
            columnsAndValues.TryGetValue("boilingPoint", out boilingPoint);
            string boilingPointString = $"{boilingPoint}" == "" ? null : $"{boilingPoint}";

            object @volatile;
            columnsAndValues.TryGetValue("@volatile", out @volatile);
            string @volatileString = $"{@volatile}" == "" ? null : $"{@volatile}";

            SQLCode.StoredProcedures.InsertWareSP($"'{id}'", $"'{name}'", amount, $"'{type}'", informationString, dangerCategoryString, flashPointString, minTempString, boilingPointString, @volatileString);
        }
    }
}
