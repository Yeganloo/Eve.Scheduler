using Eve.Scheduler.Controller;
using Eve.Scheduler.Controller.Server;
using Eve.Scheduler.Controller.TypeHandlers;
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
                logger.LogLevel = LogLevels.Journal;
                var sman = new SettingsManager(new ManagerSettings
                {
                    SettingsFolder = "Settings"
                });
                sman.RememberMe(typeof(Settings), "Settings");

                Console.WriteLine($@"Eve scheduler version({Version}).");
                logger.Log(GlobalLoggerName, "Scheduler just starts.", LogLevels.Info);
                var setting = sman.GetSettings<Settings>();
                logger.Log(GlobalLoggerName, "Settings loaded.", LogLevels.Journal);

                //TODO load cache provider.

                //TODO load handler modules.
                foreach (var handle in setting.Handlers)
                {
                    handlers.Add(handle.Type, new MessageHandler(handle.Type, new BashTypeHandler(sman), logger));
                    logger.Log(GlobalLoggerName, $"{handle.Type} handler is loaded.", LogLevels.Info);
                }
                using (var socket = new SocketController(logger, handlers))
                {
                    foreach (var ss in setting.SocketSettings)
                    {
                        socket.Add(ss);
                    }

                    string[] tokens;
                    while (running)
                    {
                        tokens = Read().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        //try
                        {
                            switch (tokens[0].ToLower())
                            {
                                case "exit":
                                    running = false;
                                    break;
                                case "exec":
                                    var msg = new Message
                                    {
                                        MessageType = tokens[1],
                                        Payload = new byte[0],
                                        Status = (byte)MessageStatus.SaveResult,
                                        Timeout = 1500,
                                        Retries = 2
                                    };
                                    for (int i = 2; i < tokens.Length; i++)
                                    {
                                        switch (tokens[i])
                                        {
                                            case "-i":
                                                msg.Identifier = tokens[++i];
                                                break;
                                            case "-T":
                                                msg.Timeout = int.Parse(tokens[++i]);
                                                break;
                                            case "-p":
                                                msg.Payload = Convert.FromBase64String(tokens[++i]);
                                                break;
                                            case "-r":
                                                msg.Retries = int.Parse(tokens[++i]);
                                                break;
                                        }
                                    }
                                    handlers[msg.MessageType].Handle(msg, msg.Retries - 1);
                                    break;
                                default:
                                    Console.WriteLine("Read Help!");
                                    break;
                            }
                        }
                        //catch(Exception e)
                        //{
                        //    Console.WriteLine("Read Help!");
                        //}
                    }
                }
            }
        }
    }
}
