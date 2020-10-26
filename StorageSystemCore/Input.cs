using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    static public class Input
    {
        /// <summary>
        /// Enum that contains the keys that are used in the software. 
        /// </summary>
        public enum Keys
        {
            Enter = 13,
            BackSpace = 8,
            UpArray = 38,
            DownArray = 40
        }

        private static ConsoleKeyInfo key;

        /// <summary>
        /// Ensures that the input system is always working by running it on another thread. 
        /// </summary>
        /// <exception cref="ThreadStateException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        public static void RunInputThread()
        {
            Thread inputThread = new Thread(InputRun);
            inputThread.Name = "Input Thread";
            try
            {
                inputThread.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// If a key is pressed, activate an event and transmit the key. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        static private void InputRun()
        {

            try
            {
                do
                {
                    if (Console.KeyAvailable)
                    {
                        key = KeyInput();
                        BufferFlush();
                    }

                } while (true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Reads and return a key without displaying it.
        /// </summary>
        /// <returns>Reads and return a key.</returns>
        private static ConsoleKeyInfo KeyInput()
        {
            return Console.ReadKey(true);
        }

        public delegate string InputStringDelegate();
        /// <summary>
        /// Collects and returns a user inputted string.
        /// </summary>
        public static InputStringDelegate GetString = getInput;
        /// <summary>
        /// Collects and returns a user inputted string.
        /// </summary>
        /// <returns>Returns a string the user has inputted.</returns>
        private static string getInput()
        {
            return Console.ReadLine();
        }

        public delegate ConsoleKeyInfo InputSingleKeyInfoDelegate();
        /// <summary>
        /// Returns a ConsoleKeyInfo, if no key is avaliable it will return the default value.
        /// </summary>
        public static InputSingleKeyInfoDelegate GetKeyInfo = KeyInfo;
        /// <summary>
        /// Returns a ConsoleKeyInfo, if no key is avaliable it will return the default value.
        /// </summary>
        /// <returns>Returns a ConsoleKeyInfo.</returns>
        private static ConsoleKeyInfo KeyInfo()
        {
            if (key == new ConsoleKeyInfo()) 
                return new ConsoleKeyInfo();
            ConsoleKeyInfo key_ = key;
            key = new ConsoleKeyInfo();
            return key_;
        }

        public delegate ConsoleKey InputSingleKeyDelegate();
        /// <summary>
        /// Returns a ConsoleKey, if no key is avaliable it will return the default value.
        /// </summary>
        public static InputSingleKeyDelegate InputSingleKey = Key;
        /// <summary>
        /// Returns a ConsoleKey, if no key is avaliable it will return the default value.
        /// </summary>
        /// <returns>Returns a ConsoleKey.</returns>
        private static ConsoleKey Key()
        {
            if (key == new ConsoleKeyInfo())
                return new ConsoleKey();
            ConsoleKey key_ = key.Key;
            key = new ConsoleKeyInfo();
            return key_;
        }

        public delegate bool KeyAvaliable();
        /// <summary>
        /// Returns true if there is a key avaliable in the buffer.
        /// </summary>
        public static KeyAvaliable IskeyAvaliable = keyAvaliable;
        /// <summary>
        /// Returns true if there is a key avaliable in the buffer
        /// </summary>
        /// <returns>Returns true if there is a key avaliable in the buffer</returns>
        private static bool keyAvaliable()
        {
            return Console.KeyAvailable;
        }

        /// <summary>
        /// Flushes the Console.Key buffer.
        /// </summary>
        private static void BufferFlush()
        {
            while (IskeyAvaliable())
                KeyInput();
        }

        public delegate bool KeyCompareDelegate(ConsoleKey pressedKey, Keys key_);
        /// <summary>
        /// Compares two keys and return true if they are the same.
        /// </summary>
        public static KeyCompareDelegate KeyCompare = KeyComparision;
        /// <summary>
        /// Compares two keys and return true if they are the same.
        /// </summary>
        /// <param name="pressedKey">The pressed key.</param>
        /// <param name="key_">The key to compare too.</param>
        /// <returns>Returns true if the keys are the samen, else false.</returns>
        private static bool KeyComparision(ConsoleKey pressedKey, Keys key_)
        {
            bool result = (int)key_ == (int)pressedKey;
            return result;
        }

    }
}
