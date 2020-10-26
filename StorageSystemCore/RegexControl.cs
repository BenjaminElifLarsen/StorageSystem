using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StorageSystemCore
{
    /// <summary>
    /// Contains functions that use Regex.
    /// </summary>
    public static class RegexControl
    {
        private static string specialSigns = @"_ \- \= \+ \. \,"; 
        private static string allowedSigns = "^[a-z A-Z 0-9" + specialSigns + "]$";
        private static Regex rgSpeical = new Regex(@"[" + specialSigns + "]{1,}");
        private static Regex rgValues = new Regex("[0-9]{1,}");
        private static Regex rgLettersLower = new Regex(@"\p{Ll}{1,}");
        private static Regex rgLettersUpper = new Regex(@"\p{Lu}{1,}");
        private static Regex rgLength = new Regex("^.{6,16}$"); 
        private static Regex rgUnallowedSigns = new Regex(@"[^a-z ^A-Z ^0-9 ^_ ^\- ^\= ^\+ ^\. ^\,]");
        
        /// <summary>
        /// Gets the specials that, at least one, need to be in the string to be valid.
        /// </summary>
        public static string GetSpecialSigns { get => specialSigns.Replace("\\",""); }

        /// <summary>
        /// Uses Regex to ensure <paramref name="text"/> only contains valid chars.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Returns true if <paramref name="text"/> only contains valid chars.</returns>
        /// <exception cref="NullReferenceException"
        public static bool IsValidCharsOnly(string text)
        {
            if (text == null)
                throw new NullReferenceException();
            return !rgUnallowedSigns.IsMatch(text);
        }

        /// <summary>
        /// Uses Regex to ensure <paramref name="text"/> is of a specific length of valid signs.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Returns true if <paramref name="text"/> is widthin a given length.</returns>
        /// <exception cref="NullReferenceException"
        public static bool IsValidLength(string text)
        {
            if (text == null)
                throw new NullReferenceException();
            return rgLength.IsMatch(text);
        }

        /// <summary>
        /// Uses Regex to ensure <paramref name="text"/> contains at least 1 number.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Returns true if <paramref name="text"/> contains a minimum of 1 number.</returns>
        /// <exception cref="NullReferenceException"
        public static bool IsValidValues(string text)
        {
            if (text == null)
                throw new NullReferenceException();
            return rgValues.IsMatch(text);
        }

        /// <summary>
        /// Uses Regex to ensure <paramref name="text"/> contains at least 1 lowercase letter.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Returns true if <paramref name="text"/> contains a minimum of 1 lowercase letter.</returns>
        /// <exception cref="NullReferenceException"
        public static bool IsValidLettersLower(string text)
        {
            if (text == null)
                throw new NullReferenceException();
            return rgLettersLower.IsMatch(text);
        }

        /// <summary>
        /// Uses Regex to ensure <paramref name="text"/> contains at least 1 uppercase letter.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Returns true if <paramref name="text"/> contains a minimum of 1 uppercase letter.</returns>
        /// <exception cref="NullReferenceException"
        public static bool IsValidLettersUpper(string text)
        {
            if (text == null)
                throw new NullReferenceException();
            return rgLettersUpper.IsMatch(text);
        }

        /// <summary>
        /// Uses Regex to ensure <paramref name="text"/> contains at least 1 special symbol.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Returns true if <paramref name="text"/> contains a minimum of 1 special symbol.</returns>
        /// <exception cref="NullReferenceException"
        public static bool IsValidSpecial(string text)
        {
            if (text == null)
                throw new NullReferenceException();
            return rgSpeical.IsMatch(text);
        }

    }



}
