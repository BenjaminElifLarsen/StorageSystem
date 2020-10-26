using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageSystemCore
{
    public static class Reporter
    {
        private static readonly string pathwayLog = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Storage System\\Log\\";
        private static readonly string filenameLog = "Log";
        private static readonly string filetypeLog = ".txt";

        private static readonly string pathwayError = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Storage System\\Error\\";
        private static readonly string filenameError = "Error";
        private static readonly string filetypeError = ".txt";

        /// <summary>
        /// Gets the save location of the log.
        /// </summary>
        public static string LogLocation { get => pathwayLog + filenameLog + filetypeLog; }

        /// <summary>
        /// Gets the save location of the error report.
        /// </summary>
        public static string ErrorLocation { get => pathwayError + filenameError + filetypeError; }

        /// <summary>
        /// Wrties <paramref name="log"/> to the log file.
        /// </summary>
        /// <param name="log">Message to log.</param>
        /// <exception cref="IOException"></exception>
        public static void Log(string log)
        {
            try
            {
                DateTime time = DateTime.Now;
                string timePoint = $"{time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{time.Second}-{time.Millisecond}";
                CreateFolder(pathwayLog);
                CreateFile(pathwayLog, filenameLog + filetypeLog);
                string pathFile = Path.Combine(pathwayLog, filenameLog + filetypeLog);
                while (FileInUse(pathwayLog, filenameLog, filetypeLog)) ;
                using (StreamWriter file = new StreamWriter(pathFile, true))
                {
                    file.WriteLine(timePoint);
                    file.Write(Environment.NewLine);
                    file.WriteLine(log);
                    file.Write(Environment.NewLine);
                    file.WriteLine("".PadLeft(100, '-'));
                    file.Write(Environment.NewLine);
                    file.Write(Environment.NewLine);
                    file.Close();
                }
            }
            catch (IOException e)
            {
                Report(e);
            }
        }

        /// <summary>
        /// Wrties <paramref name="e"/> to the error file. 
        /// </summary>
        /// <param name="e">The error to record.</param>
        public static void Report(Exception e)
        {
            DateTime time = DateTime.Now;
            string timePoint = $"{time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{time.Second}-{time.Millisecond}";
            CreateFolder(pathwayError); 
            CreateFile(pathwayError, filenameError + filetypeError);
            string pathFile = Path.Combine(pathwayError, filenameError + filetypeError);
            while (FileInUse(pathwayError, filenameError, filetypeError)) ;
            using (StreamWriter file = new StreamWriter(pathFile, true))
            {
                file.WriteLine(timePoint);
                file.Write(Environment.NewLine);
                file.WriteLine(e.Source);
                file.Write(Environment.NewLine);
                file.WriteLine(e.Message);
                file.Write(Environment.NewLine);
                file.WriteLine(e.TargetSite);
                file.Write(Environment.NewLine);
                file.WriteLine(e);
                file.Write(Environment.NewLine);
                file.WriteLine("".PadLeft(100, '-'));
                file.Write(Environment.NewLine);
                file.Write(Environment.NewLine);
                file.Close();
            }
        }

        /// <summary>
        /// Creates a folder with the given <paramref name="path"/> if it does not exist.
        /// </summary>
        /// <param name="path">The pathway of the folder</param>
        private static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Creates a file with the name and type in <paramref name="file"/> located at <paramref name="path"/> if it does not exist
        /// </summary>
        /// <param name="path">The pathway of the file</param>
        /// <param name="file">The filename and type</param>
        private static void CreateFile(string path, string file)
        {
            string pathFile = Path.Combine(path, file);
            if (!File.Exists(pathFile))
                using (FileStream fs = File.Create(pathFile)) ;
        }

        /// <summary>
        /// Checks if a file is in used by another processed. Returns true if it is, else false.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="filetype">The file type.</param>
        /// <returns>Returns true if the file is in use, else false.</returns>
        private static bool FileInUse(string path, string filename, string filetype)
        {
            try
            {
                string pathFile = Path.Combine(path, filename + filetype);
                using (StreamWriter file = new StreamWriter(pathFile, true)) ;
                return false;

            }
            catch
            {
                return true;
            }
        }

    }
}
