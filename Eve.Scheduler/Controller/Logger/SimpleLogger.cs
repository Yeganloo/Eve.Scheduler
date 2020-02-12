using System;
using System.Collections.Generic;
using System.IO;

namespace Eve.Scheduler.Controller
{
    public class SimpleLogger : IDisposable
    {
        private string BaseAddress;
        private Dictionary<string, Stream> Writers;
        private ILogProvider logProvider;
        private FileMode _FileMode;

        public LogLevels LogLevel { get; set; }

        public SimpleLogger(string logFolder, ILogProvider logProvider, FileMode fileMode = FileMode.Append)
        {
            BaseAddress = logFolder;
            if (!Directory.Exists(BaseAddress))
                Directory.CreateDirectory(BaseAddress);
            Writers = new Dictionary<string, Stream>();
            this.logProvider = logProvider;
            _FileMode = fileMode;
        }

        public void CreateLogger(string name)
        {
            var str = File.Open($"{BaseAddress}{Path.DirectorySeparatorChar}{name}.{logProvider.Extention}", _FileMode, FileAccess.Write, FileShare.Read);
            logProvider.Init(str);
            Writers.Add(name, str);
        }

        public void Log(string loggerName, object Log, LogLevels level)
        {
            if (level < LogLevel)
                return;
            try
            {
                var writer = Writers[loggerName];
                lock (writer)
                {
                    writer.Write(logProvider.GetByte(Log));
                    writer.Flush();
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("Logger not found", ex);
            }
        }


        public void Dispose()
        {
            foreach (var str in Writers.Values)
            {
                logProvider.Dispose(str);
                str.Dispose();
            }
        }
    }
}
