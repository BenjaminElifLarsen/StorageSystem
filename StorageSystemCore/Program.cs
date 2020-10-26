using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{

    class Program
    {
        /// <summary>
        /// Contains exit codes for the software. 
        /// </summary>
        public enum ExitCode : int
        {
            Ordinary = 0,
            OutOfMemory = 8,
            UnknownException = 16000
        }

        static void Main(string[] args) 
        {
            Menu menu = new Menu();
            try
            {
                Input.RunInputThread();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Program encountered an error under upstart: {e.Message}");
                Support.WaitOnKeyInput();
                int exitCode;
                if (e is OutOfMemoryException)
                    exitCode = (int)ExitCode.OutOfMemory;
                else
                    exitCode = (int)ExitCode.UnknownException;
                Environment.Exit(exitCode);
            }
            menu.DatabaseSelectionMenu();
            new WareCreator(Publisher.PubWare); 
            menu.MainMenu();
        }

    }

    
   
    

    






}
