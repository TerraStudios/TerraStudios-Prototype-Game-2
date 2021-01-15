//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.IO;

namespace DebugTools
{
    /// <summary>
    /// Determines the type of logging to be used
    /// </summary>
    public enum LogLevel
    {
        Info, Warning, Error
    }

    /// <summary>
    /// Class used for various debugging methods
    /// </summary>
    internal static class Log
    {
        /// <summary>
        /// Determines whether any calls to <see cref="ConsoleLog(LogLevel, string)"/> should actually print to the console. 
        /// </summary>
        public static bool DEBUG_MODE = false;

        /// <summary>
        /// Sends a message to the Unity console. Uses <see cref="LogLevel.Info"/> as a default <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="message">The message that will be sent to the console</param>
        public static void LogConsole(string message)
        {
            LogConsole(LogLevel.Info, message);
        }

        /// <summary>
        /// Sends a message to the Unity console with a <see cref="LogLevel"/> determining the type of log.
        /// </summary>
        /// <param name="type">The <see cref="LogLevel"/> the message should be sent as. Each <see cref="LogLevel"/> represents a different Unity logging type, e.g. Error or Warning.</param>
        /// <param name="message">The message that will be sent to the console</param>
        public static void LogConsole(LogLevel type, string message)
        {
            if (DEBUG_MODE)
            {
                switch (type)
                {
                    case LogLevel.Info:
                        UnityEngine.Debug.Log(message);
                        break;
                    case LogLevel.Warning:
                        UnityEngine.Debug.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        UnityEngine.Debug.LogError(message);
                        break;

                }
            }
        }

        /// <summary>
        /// Logs a message into a given text file with a prefix of [Info] by default.
        /// </summary>
        /// <param name="filePath">The filepath the text file is located in</param>
        /// <param name="message">The message that will be sent to the console</param>
        /// <exception cref="FileNotFoundException">if the file path does not contain a valid text file</exception>
        public static void LogFile(string filePath, string message)
        {
            LogFile(filePath, LogLevel.Info, message);
        }

        /// <summary>
        /// Logs a message into a given text file with a prefix depending on the <see cref="LogLevel"/> provided.
        /// </summary>
        /// <param name="filePath">The filepath the text file is located in</param>
        /// <param name="type">The prefix assigned to the message</param>
        /// <param name="message">The message that will be sent to the console</param>
        /// <exception cref="FileNotFoundException">if the file path does not contain a valid text file</exception>
        public static void LogFile(string filePath, LogLevel type, string message)
        {
            if (!File.Exists(filePath))
            {
                LogConsole(LogLevel.Error, "Encountered an exception while logging to a text file.");
                throw new FileNotFoundException($"The path {filePath} does not contain a suitable text file for logging.");
            }

            string textToAppend = message + Environment.NewLine;
            File.AppendAllText(filePath, textToAppend);
        }
    }
}
