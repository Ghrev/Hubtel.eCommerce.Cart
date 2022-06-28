using System;
using System.IO;
using Hubtel.eCommerce.Cart.Models.ConfigModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hubtel.eCommerce.Cart.Data.Utility
{
    public class Logger
    {
        public static bool VerboseLogging { get; set; }
        private static readonly object Locker = new object();
        private string _infoLogDirectory;
        private string _errorLogDirectory;
        private string _warningLogDirectory;

        public Logger()
        {
            var logDirectories = new LogDirectory
            {
                Error = "./Error_Test",
                Info = "./Info_Test",
                Warning = "./Warning_Test"
            };

            SetDirectories(logDirectories);
        }

        public Logger(IOptions<LogDirectory> options)
        {
            var logDirectories = options.Value;
            SetDirectories(logDirectories);
        }

        private void SetDirectories(LogDirectory directories)
        {
            _errorLogDirectory = directories.Error;
            _infoLogDirectory = directories.Info;
            _warningLogDirectory = directories.Warning;
        }

        public void LogError(Exception x)
        {
            try
            {
                var dir = Path.Combine(_errorLogDirectory, DateTime.Now.ToString("yyyy_MM_dd"));
                CheckDir(dir);
                dir = Path.Combine(dir, DateTime.Now.ToString("HH"));
                CheckDir(dir);

                var errorMessage = $"{DateTime.Now.ToString("HH:mm:ss")} || {x}  \r\n {x.StackTrace} \n";

                var logFileName = string.Format("Hubtel_e_Commerce_Error_{0}_{1}.log", DateTime.Now.ToString("yyyy_MM_dd"), DateTime.Now.ToString("HH"));
                var logFilePath = Path.Combine(dir, logFileName);

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(errorMessage);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void LogError(string message)
        {
            try
            {
                var dir = Path.Combine(_errorLogDirectory, DateTime.Now.ToString("yyyy_MM_dd"));
                CheckDir(dir);
                dir = Path.Combine(dir, DateTime.Now.ToString("HH"));
                CheckDir(dir);

                var errorMessage = $"{DateTime.Now.ToString("HH:mm:ss")} || {message} \n";

                var logFileName = string.Format("Hubtel_e_Commerce_Error_{0}_{1}.log", DateTime.Now.ToString("yyyy_MM_dd"), DateTime.Now.ToString("HH"));
                var logFilePath = Path.Combine(dir, logFileName);

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(errorMessage);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void LogInfo(string message)
        {
            try
            {
                var dir = Path.Combine(_infoLogDirectory, DateTime.Now.ToString("yyyy_MM_dd"));
                CheckDir(dir);
                dir = Path.Combine(dir, DateTime.Now.ToString("HH"));
                CheckDir(dir);

                var infoMessage = $"{DateTime.Now.ToString("HH:mm:ss")} || {message}\n";

                var logFileName = string.Format("Hubtel_e_Commerce_Info_{0}_{1}.log", DateTime.Now.ToString("yyyy_MM_dd"), DateTime.Now.ToString("HH"));
                var logFilePath = Path.Combine(dir, logFileName);

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(infoMessage);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void LogWarning(string message)
        {
            lock (Locker)
            {
                try
                {
                    var dir = Path.Combine(_warningLogDirectory, DateTime.Now.ToString("yyyy_MM_dd"));
                    CheckDir(dir);
                    dir = Path.Combine(dir, DateTime.Now.ToString("HH"));
                    CheckDir(dir);

                    var warningMessage = $"{DateTime.Now.ToString("HH:mm:ss")} || {message} \n";

                    var logFileName = string.Format("Hubtel_e_Commerce_Warning_{0}_{1}.log", DateTime.Now.ToString("yyyy_MM_dd"), DateTime.Now.ToString("HH"));
                    var logFilePath = Path.Combine(dir, logFileName);

                    using (StreamWriter sw = File.AppendText(logFilePath))
                    {
                        sw.WriteLine(warningMessage);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void CheckDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
