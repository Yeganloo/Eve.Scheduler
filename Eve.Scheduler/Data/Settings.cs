using Eve.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler
{
    public class Settings
    {
        public const string YamlLogType = "Yaml";
        public const string MessagePackLogType = "MessagePack";
        public const string JsonLogType = "Json";

        public string LogType { get; set; }


    }
}
