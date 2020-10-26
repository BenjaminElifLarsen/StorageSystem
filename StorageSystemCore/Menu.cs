using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// Class that contain the main part of the menu system.
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Runs the main menu of the storage system.
        /// </summary>
        public void MainMenu() //put WareCreateMenu and WareChangeMenu into a sub menu. 
        {
            string title = "Main Menu";
            string[] menuOptions = new string[] {"Basic Storage View", "Expanded Storage View", "Add Ware","Change Ware", "Exit", /*" SQL Test"*/ };
            do
            {
                byte answer = VisualCalculator.MenuRun(menuOptions, title);
                switch (answer)
                {
                    case 0:
                        WareViewBasicMenu();
                        break;

                    case 1:
                        WareViewMenu();
                        break;

                    case 2:
                        WareCreateMenu();
                        break;

                    case 3:
                        WareChangeMenu();
                        break;

                    case 4:
                        Publisher.PubWare.RemoveWareCreator();
                        Environment.Exit((int)Program.ExitCode.Ordinary);
                        break;

                    case 5:
                        DatabaseSelectionMenu();
                        break;
                }
            } while (true);

        }

        /// <summary>
        /// Runs the menu that allows for changes to a ware. 
        /// </summary>
        private void WareChangeMenu()
        {
            bool run = true;
            string title = "Ware Change Menu";
            string[] options = new string[] { "Remove Ware", "Add To Ware", "Remove From Ware", "Modify Ware", "Back" };
            do
            {
                byte response = VisualCalculator.MenuRun(options, title);
                switch (response)
                {
                    case 0:
                        WareRemoveMenu(); 
                        break;

                    case 1:
                        WareAddAmountMenu();
                        break;

                    case 2:
                        WareRemoveAmountMenu();
                        break;

                    case 3:
                        WareModifyMenu();
                        break;

                    case 4:
                        run = false;
                        break;
                }
            } while (run);
        }

        /// <summary>
        /// Runs the menu for removing a ware. 
        /// </summary>
        private void WareRemoveMenu()
        {
            string ID = CollectID();
            if (Support.IDExist(ID, SQLCode.SQLControl.DatabaseInUse))
                if(WareModifier.RemoveWare(ID))
                {
                    ProgressInformer("{0} was removed.", ID);
                }
                else
                {
                    ProgressInformer("{0} was not removed.", ID);
                }
            else
            {
                OutPut.DisplayMessage("ID does not exist");
                Support.WaitOnKeyInput();
            }

            void ProgressInformer(string message, string part)
            {
                OutPut.FullScreenClear();
                string text = String.Format(message, part);
                OutPut.DisplayMessage(text);
                Support.WaitOnKeyInput();
            }
        }

        /// <summary>
        /// Runs the menu to modify a ware. 
        /// </summary>
        private void WareModifyMenu()
        {
            string ID = CollectID();
            if (Support.IDExist(ID, SQLCode.SQLControl.DatabaseInUse))
                WareModifier.ModifyWare(ID);
            else
            {
                OutPut.DisplayMessage("ID does not exist");
                Support.WaitOnKeyInput();
            }
        }

        /// <summary>
        /// Runs the menu for adding any amount to a ware. 
        /// </summary>
        private void WareAddAmountMenu()
        {
            string ID = CollectID();
            if (Support.IDExist(ID, SQLCode.SQLControl.DatabaseInUse))
                WareModifier.AddToWare(ID,CollectAmount());
            else
            {
                OutPut.DisplayMessage("ID does not exist");
                Support.WaitOnKeyInput();
            }
        }

        /// <summary>
        /// Runs the menu for removing any amount from a ware. 
        /// </summary>
        private void WareRemoveAmountMenu()
        {
            string ID = CollectID();
            if (Support.IDExist(ID, SQLCode.SQLControl.DatabaseInUse)) 
                WareModifier.RemoveFromWare(ID, CollectAmount());
            else
            {
                OutPut.DisplayMessage("ID does not exist");
                Support.WaitOnKeyInput();
            }
        }

        /// <summary>
        /// Asks the user to enter an amount and returns it.
        /// </summary>
        /// <returns></returns>
        private int CollectAmount() 
        {
            return Support.CollectValue("Enter Amount");
        }

        /// <summary>
        /// Asks the user to enter an ID and returns it.
        /// </summary>
        /// <returns>Returns the inputted ID.</returns>
        private string CollectID()
        {
            return Support.CollectString("Enter ID");
        }

        /// <summary>
        /// Starts the creation of a new ware.
        /// </summary>
        private void WareCreateMenu()
        {
            Publisher.PubWare.CreateWare();
        } 

        /// <summary>
        /// Runs the code to view the basic information of all wares.
        /// </summary>
        private void WareViewBasicMenu()
        {
            if(!SQLCode.SQLControl.DatabaseInUse)
                VisualCalculator.WareDisplay(WareInformation.GetWareInformation());
            else
            { 
                try 
                {
                    List<string[]> informationReady = SQLDataCollection();
                    VisualCalculator.WareDisplay(informationReady);
                }
                catch (Exception e)
                {
                    Support.ErrorHandling(e, $"Could not collect data: {e.Message}");
                }
            }
            Support.WaitOnKeyInput();

            List<string[]> SQLDataCollection(){
                List<List<string>> information = SQLCode.SQLControl.GetValuesAllWare(new string[] { "name", "id", "amount", "type" });
                List<string[]> informationReady = new List<string[]>();
                foreach (List<string> arrayData in information)
                    informationReady.Add(arrayData.ToArray());
                return informationReady;
            }
        }

        /// <summary>
        /// Runs the code that allows the user to view specific information of all wares.
        /// </summary>
        private void WareViewMenu() 
        {
            Console.Clear();
            List<string> searchAttributes = WareInformation.FindAllSearchableAttributesNames(SQLCode.SQLControl.DatabaseInUse);
            if (!SQLCode.SQLControl.DatabaseInUse) //get attributes
                searchAttributes.Add("Type");
            else
                searchAttributes.Add("type");
            searchAttributes.Add("Done");
            List<string> selectedAttributes = new List<string>(); 
            byte selected;
            do //select attributes to view
            {
                selected = VisualCalculator.MenuRun(searchAttributes.ToArray(), "Select Information To Find");
                if (selected != searchAttributes.Count - 1 && !selectedAttributes.Contains(searchAttributes[selected]))
                    selectedAttributes.Add(searchAttributes[selected]);
            } while (selected != searchAttributes.Count-1);
            List<Dictionary<string, object>> attributesAndValues;
            if (selectedAttributes.Count != 0)
            { //get the data and display it
                if (!SQLCode.SQLControl.DatabaseInUse) { 
                    try 
                    { 
                        attributesAndValues = WareInformation.GetWareInformation(selectedAttributes);
                        VisualCalculator.WareDisplay(selectedAttributes, attributesAndValues);
                    }
                    catch (NullReferenceException e)
                    {
                        Support.ErrorHandling(e, $"No attibutes were selected. {e.Message}");
                    }
                }
                else
                {
                    try 
                    { 
                        List<List<string>> wareValues = SQLCode.SQLControl.GetValuesAllWare(selectedAttributes.ToArray()); 
                        VisualCalculator.WareDisplay(selectedAttributes.ToArray(), wareValues);
                    }
                    catch (Exception e)
                    {
                        Support.ErrorHandling(e, $"Encountered an error: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Function used to set database (or no database).
        /// </summary>
        public void DatabaseSelectionMenu() 
        {
            string[] options = new string[] { "Window login Authentication", "SQL Server Authentication", "No SQL Database" };
            string[] sqlInfo;
            string firstConnection;
            
            bool run = true;
            Reporter.Log("Program starting");
            do
            {
                byte answer = VisualCalculator.MenuRun(options, "Database");
                switch (answer) 
                {
                    case 0:
                        sqlInfo = new string[2];
                        sqlInfo[0] = Support.CollectString("Enter Servername");
                        sqlInfo[1] = Support.CollectString("Enter database");
                        if (!ShallDatabaseInitialiseMenu())
                        {
                            try
                            {
                                SQLCode.SQLControl.DataBase = sqlInfo[1];
                                firstConnection = SQLCode.SQLControl.CreateConnectionString(sqlInfo[0], "master");
                                run = !SQLCode.SQLControl.InitalitionOfDatabase(sqlInfo, firstConnection, true);
                                SQLCode.SQLControl.DatabaseInUse = true;
                            }
                            catch (Exception e)
                            {
                                Support.ErrorHandling(e, $"Encounted an error: {e.Message}");
                                run = true;
                            }
                        }
                        else
                            try
                            {
                                SQLCode.SQLControl.DataBase = sqlInfo[1];
                                run = !SQLCode.SQLControl.CreateConnection(sqlInfo, true);
                                SQLCode.SQLControl.DatabaseInUse = true;
                            }
                            catch (Exception e)
                            {
                                Support.ErrorHandling(e, $"Encounted an error: {e.Message}");
                                run = true;
                            }
                        break;

                    case 1:
                        sqlInfo = new string[4];
                        sqlInfo[0] = Support.CollectString("Enter Servername");
                        sqlInfo[1] = Support.CollectString("Enter SQL Username");
                        sqlInfo[2] = Support.HiddenText("Enter Password");
                        sqlInfo[3] = Support.CollectString("Enter database");
                        if (!ShallDatabaseInitialiseMenu()) 
                        {
                            try
                            {
                                SQLCode.SQLControl.DataBase = sqlInfo[3];
                                firstConnection = SQLCode.SQLControl.CreateConnectionString(sqlInfo[0], sqlInfo[1], sqlInfo[2], "master");
                                run = !SQLCode.SQLControl.InitalitionOfDatabase(sqlInfo, firstConnection, false);
                                SQLCode.SQLControl.DatabaseInUse = true;
                            }
                            catch (Exception e)
                            {
                                Support.ErrorHandling(e, $"Encounted an error: {e.Message}");
                                run = true;
                            }
                        }
                        else
                            try {
                                SQLCode.SQLControl.DataBase = sqlInfo[3];
                                run = !SQLCode.SQLControl.CreateConnection(sqlInfo, false);
                                SQLCode.SQLControl.DatabaseInUse = true;
                            }
                            catch (Exception e)
                            {
                                Support.ErrorHandling(e, $"Encounted an error: {e.Message}");
                                run = true;
                            }

                        break;

                    case 2:
                        WareInformation.AddWareDefault();
                        SQLCode.SQLControl.DatabaseInUse = false;
                        break;
                }
                if (answer == options.Length - 1)
                    run = false;
                if(run == true)
                {
                    Console.Clear();
                    Console.WriteLine("Could not establish connection to the database");
                    Support.WaitOnKeyInput();
                }

            } while (run);
        }

        
        /// <summary>
        /// Small menu that asks the user if the user wants to initialise database creation.
        /// </summary>
        /// <returns>Returns true if the user selects yes to the database initialisation, else false.</returns>
        private bool ShallDatabaseInitialiseMenu()
        {
            string[] options = new string[] {"Yes","No" };
            byte answer = VisualCalculator.MenuRun(options, "Initialise Database Creation?");
            return answer == 1;
        }


    }
}
