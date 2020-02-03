using Eve.Scheduler.Controller;
using Eve.Scheduler.Controller.Log;
using Eve.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler
{
    class Program
    {
        public const string Version = "0.0.1-develop";
        public const string GlobalLoggerName = "global";

        private static string Read()
        {
            Console.Write("/> ");
            return Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Dictionary<string, MessageHandler> handlers = new Dictionary<string, MessageHandler>();
            bool running = true;
            using (var logger = new SimpleLogger("logs", new YamlProvider()))
            {
                logger.CreateLogger(GlobalLoggerName);
                //TODO Read Log level from parameters.
                logger.LogLevel = LogLevels.Debug;
                var sman = new SettingsManager(new ManagerSettings
                {
                    SettingsFolder = "Settings"
                });
                sman.RememberMe(typeof(Settings), "Settings");

                Console.WriteLine($@"Eve scheduler version({Version}).");
                logger.Log(GlobalLoggerName, "Scheduler just starts.", LogLevels.Debug);
                var setting = sman.GetSettings<Settings>();
                logger.Log(GlobalLoggerName, "Settings loaded.", LogLevels.Debug);

                //TODO load cache provider.

                //TODO load builtin handlers.

                //TODO load handler modules.
                foreach (var handle in setting.Handlers)
                {
                    handlers.Add(handle.Type, new MessageHandler(handle.Type, (sman, msg) =>
                    {
                        return Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(msg));
                    }, sman));
                    logger.Log(GlobalLoggerName, $"{handle.Type} handler is loaded.", LogLevels.Debug);
                }

                //TODO Load socket listenner.

                string Command;
                while (running)
                {
                    Command = Read();
                    switch (Command.ToLower())
                    {
                        case "exit":
                            running = false;
                            break;
                        case "test":
                            handlers["test"].Handle(new Message
                            {
                                Identifier = "001",
                                MessageType = "test",
                                Payload = new byte[0],
                                Status = (byte)MessageStatus.SaveResult,
                                Timeout = 1500,
                                Retries = 2
                            }, 2);
                            break;
                        default:
                            Console.WriteLine("Read Help!");
                            break;
                    }
                }
            }
        }
    }
}
