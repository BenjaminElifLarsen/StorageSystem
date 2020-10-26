using System;
using System.Collections.Generic;
using System.Text;

namespace StorageSystemCore
{

    /// <summary>
    /// Class for outputs
    /// </summary>
    public class OutPut 
    {
        public delegate void WriteOutDelegate(string message, bool newLine = false);
        /// <summary>
        /// Displays a basic message and adds either a newline or not.
        /// </summary>
        public static WriteOutDelegate DisplayMessage = WriteOutMessage; //consider having one for single char output
        /// <summary>
        /// Writes out to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="newLine">True if a newline should be added.</param>
        private static void WriteOutMessage(string message, bool newLine = false)
        {
            Console.Write(message);
            if (newLine)
                Console.WriteLine();
        }

        public delegate void WriteOutDelegateTitle(string message, VisualCalculator.Colours colour, bool newLine = false);
        /// <summary>
        /// Displays a message with either a newline or not with a specific colour. 
        /// </summary>
        public static WriteOutDelegateTitle DisplayColouredMessage = WriteOutMessage;
        /// <summary>
        /// Writes out to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="newLine">True if a newline should be added.</param>
        /// <param name="colour">The colour to display the message in.</param>
        private static void WriteOutMessage(string message, VisualCalculator.Colours colour, bool newLine = false)
        {
            Console.ForegroundColor = (ConsoleColor)(int)colour;
            Console.Write(message);
            if (newLine)
                Console.WriteLine();
        }

        public delegate void WriteOutDelegateComplex(string message, int x, int y, VisualCalculator.Colours colour1, bool newLine = false);
        /// <summary>
        /// Displays a messages with either a newline or not with a specific colour at a specific x,y point
        /// </summary>
        public static WriteOutDelegateComplex DisplayColouredMessageAtLocation = WriteOutMessage;
        /// <summary>
        /// Writes out to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="newLine">True if a newline should be added.</param>
        /// <param name="colour">The colour to display the message in.</param>
        /// <param name="x">The x coordinate to write at.</param>
        /// <param name="y">The y coordinate to write at.</param>
        private static void WriteOutMessage(string message, int x, int y, VisualCalculator.Colours colour, bool newLine = false)
        {
            Console.ForegroundColor = (ConsoleColor)(int)colour;
            Console.SetCursorPosition(x, y);
            Console.Write(message);
            if (newLine)
                Console.WriteLine();
        }

        public delegate void ClearPartDelegate(byte length, int y, int x = 0);
        /// <summary>
        /// Clears a specific line from the left side up to and with the length given.
        /// </summary>
        public static ClearPartDelegate ClearPart = LineScreenClear;
        /// <summary>
        /// Clears part of the screen.
        /// </summary>
        /// <param name="length">The amount of space to remove.</param>
        /// <param name="y">The y coordinate to remove at.</param>
        /// <param name="x">The x coordinate to start remove at.</param>
        private static void LineScreenClear(byte length, int y, int x = 0)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
            Console.Write(" ".PadLeft(length));
        }

        public delegate void FullClearDelegate();
        /// <summary>
        /// Clears all of the displayed output. 
        /// </summary>
        public static FullClearDelegate FullScreenClear = FullClear;
        /// <summary>
        /// Clears the entire visual display.
        /// </summary>
        private static void FullClear()
        {
            Console.Clear();
        }

        public delegate int ScreenWidthDelegate();
        /// <summary>
        /// Gets and returns the width of the UI.
        /// </summary>
        public static ScreenWidthDelegate UIWidth = ScreenWidth;
        /// <summary>
        /// Gets the width of the UI.
        /// </summary>
        /// <returns>Returns the width of the UI.</returns>
        private static int ScreenWidth()
        {
            return Console.WindowWidth;
        }

        public delegate int[] CursorLocationDelegate();
        /// <summary>
        /// Returns an int array with the current location of the cursor. Index 0 being x, index 1 being y.
        /// </summary>
        public static CursorLocationDelegate CursorLocation = CurrentCursorLocation;
        /// <summary>
        /// Returns an int array with the current location of the cursor. Index 0 being x, index 1 being y.
        /// </summary>
        /// <returns></returns>
        private static int[] CurrentCursorLocation()
        {
            return new int[] { Console.CursorLeft, Console.CursorTop };
        }

        public delegate void MoveCursorDelegate(int x = 0, int y = 0);
        /// <summary>
        /// Moves the current x coordinate of the cursor with the amount given in the parameters or the maximum/minimum values in case of over/underflow.
        /// </summary>
        public static MoveCursorDelegate MoveCursor = CursorMove;
        /// <summary>
        /// Moves the current x coordinate of the cursor with the amount given in <paramref name="xAmount"/> and <paramref name="yAmount"/> or the maximum/minimum values in case of over/underflow.
        /// </summary>
        /// <param name="xAmount">The amount to move the cursor with on the x axi.</param>
        /// <param name="yAmount">The amount to move the cursor with on the y axi.</param>
        private static void CursorMove(int xAmount = 0, int yAmount = 0)
        {
            if(yAmount != 0) { 
                if (Console.CursorTop + yAmount < Console.WindowHeight - 1 && Console.CursorTop + yAmount >= 0)
                    Console.CursorTop += yAmount;
                else
                {
                    if (yAmount > 0)
                        Console.CursorTop = Console.WindowHeight - 1;
                    else
                        Console.CursorTop = 0;
                }
            }
            if (xAmount != 0)
            {
                if (Console.CursorLeft + xAmount < Console.WindowWidth - 1 && Console.CursorLeft + xAmount >= 0)
                    Console.CursorLeft += xAmount;
                else
                {
                    if (yAmount > 0)
                        Console.CursorLeft = Console.WindowWidth - 1;
                    else
                        Console.CursorLeft = 0;
                }
            }
        }


    }
}
