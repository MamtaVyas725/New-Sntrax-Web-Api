using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace EnttlOrchestrationLayer.Utilities
{
    /// <summary>
    /// Utility method to log erros 
    /// </summary>
    public static class CLogger
    {
        public static string Path { get; set; }
        public static string Filename { get; set; }

        public static bool EnableLogFile { get; set; }
        

        public static ILogger _logger;

        public static IConfiguration _rootObjectCommon;

        /// <summary>
        /// Method to log error information in a file
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            try
            {
                if (EnableLogFile)
                {
                    String FullPath = Path + Filename;

                    if (!Directory.Exists(Path))
                        Directory.CreateDirectory(Path);

                    StreamWriter sw;
                    if (!File.Exists(FullPath))
                    { sw = File.CreateText(FullPath); }
                    else
                    { sw = File.AppendText(FullPath); }

                    LogWrite(message, sw);

                    sw.Flush();
                    sw.Close();
                }

                _logger.LogInformation(message);

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        /// <summary>
        /// Method to Delete Older log files
        /// </summary>
        public static void PurdgeOlderLogFiles()
        {
            try
            {
                var files = Directory.GetFiles(Path);

                int purdgeHistoryDays = Convert.ToInt32(Environment.GetEnvironmentVariable("PurdgeLogAgeInDays"));

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddDays(-purdgeHistoryDays))
                        fi.Delete();
                }
            }
            catch (Exception ex)
            {
                LogInfo("Unable to delete Logfiles in path: " + Path);
                LogInfo(ex.Message);
            }

        }

        /// <summary>
        /// Methid used inside LogInfo() methid to write the logMessage in a file
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="w"></param>
        private static void LogWrite(string logMessage, StreamWriter w)
        {
            w.WriteLine("{0}", logMessage);
        }

    }
}
