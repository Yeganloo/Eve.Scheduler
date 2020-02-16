using Eve.Caching;
using Eve.Scheduler.Controller.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Eve.Scheduler.Controller
{
    public class MessageHandler
    {
        private const int BlockSize = 1800;
        private const int ScheduleWindow = 60000;
        private long CurrentBlockTime;
        private IEnumerable<Message> CurrentBlock;
        private static readonly DateTime UnixStartTime = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        private double UnixUTCNow
        {
            get
            {
                return DateTime.UtcNow.Subtract(UnixStartTime).TotalSeconds;
            }
        }
        private Timer Checker;
        private ICacheProvider<string, Message> cache;
        private readonly TypeHandlerBase Handler;
        private readonly SimpleLogger _Logger;
        public string MessageType { get; }
        public string HistoryFile { get; }

        public MessageHandler(TypeHandlerBase handler, SimpleLogger logger, ICacheProvider<string, Message> cacheProvider = null)
        {
            cache = cacheProvider ?? new SimpleCacheProvider<Message>();
            Handler = handler;
            Checker = new Timer(40000);
            Checker.AutoReset = true;
            Checker.Elapsed += Checker_Elapsed;
            MessageType = handler.MessageType;
            _Logger = logger;
            _Logger.CreateLogger(MessageType);
            _Logger.CreateLogger(HistoryFile = $"{MessageType}_History");
        }

        private void Checker_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TODO get 1 or 2 recent blocks to process
        }

        public void Handle(Message message)
        {
            if (string.IsNullOrEmpty(message.MessageType))
                throw new ArgumentException("Message Type can not be null!");
            if (message.MessageType != MessageType)
                throw new InvalidOperationException($"Can not handle a '{message.MessageType}' with a '{MessageType}' handler!");
            if (message.Options != null)
            {
                if (message.Options.ExpiresDateTime.HasValue && message.Options.ExpiresDateTime < UnixUTCNow - 1)
                {
                    _Logger.Log(MessageType, new { Message = message, Exception = "Request Expierd!" }, LogLevels.Fatal);
                    return;
                }
                int delay;
                if (message.Options.StartDateTime.HasValue && (delay = (int)(message.Options.StartDateTime - UnixUTCNow)) > 0)
                {
                    ScheduleForLater(delay, message);
                    return;
                }
            }
            //TODO read default retries settings.
            Handle(message, message.Options == null ? 2 : message.Options.Retries);
        }

        private void Handle(Message message, int retries)
        {
            try
            {
                //TODO handle timeout.
                var result = Handler.Handle(message);
                if (!string.IsNullOrEmpty(message.Identifier))
                    _Logger.Log(HistoryFile, new { Message = message, Result = Convert.ToBase64String(result) }, LogLevels.Force);
                if (message.Options != null && message.Options.Period > 0)
                    ScheduleForLater(message.Options.Period, message);
            }
            catch (Exception e)
            {
                _Logger.Log(MessageType, new { Retry = retries, Message = message, Exception = e.ToLog("Failed to handle message!") }, LogLevels.Error);
                if (retries > 0)
                    Handle(message, retries - 1);
            }
        }

        private async Task ScheduleForLater(int delay, Message message)
        {
            //Schedule a thread whithin 5 minutes
            if (delay < 300000)
            {
                await Task.Run(() =>
                {
                    Task.Delay(delay);
                    Handle(message);
                });
            }
            else
            {
                //TODO queue jobs to cache later
            }
        }

    }
}
