using System;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;

namespace HtmlPictureTableCreator
{
    public static class Logger
    {
        /// <summary>
        /// Contains the NLog logger
        /// </summary>
        private static readonly NLog.Logger Log = LogManager.GetLogger("Logger");

        /// <summary>
        /// Writes an info message
        /// </summary>
        /// <param name="message">The info message</param>
        public static void Info(string message)
        {
            WriteLog(LogLevel.Info, message);
        }
        /// <summary>
        /// Writes an error message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">The exception</param>
        /// <param name="memberName">The caller member name (automatically filled)</param>
        /// <param name="filepath">The filepath (automatically filled)</param>
        /// <param name="linenumber">The line number (automatically filled)</param>
        public static void Error(string message, Exception ex, [CallerMemberName] string memberName = "",
            [CallerFilePath] string filepath = "", [CallerLineNumber] int linenumber = 0)
        {
            WriteLog(LogLevel.Error, message, memberName, filepath, linenumber, ex);
        }

        private static void WriteLog(LogLevel logLevel, string message, string memberName = "", string filepath = "",
            int linenumber = 0, Exception ex = null)
        {
            var stringBuilder = new StringBuilder(message);

            if (!string.IsNullOrEmpty(memberName))
                stringBuilder.AppendLine($"Message: {message}");

            if (!string.IsNullOrEmpty(filepath))
                stringBuilder.AppendLine($"Filepath: {filepath}");

            if (linenumber != 0)
                stringBuilder.AppendLine($"Linenumber: {linenumber}");

            Log.Log(new LogEventInfo
            {
                Level = logLevel,
                Message = stringBuilder.ToString(),
                Exception = ex
            });
        }
    }
}
