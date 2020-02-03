using Eve.Caching;
using Eve.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
        private readonly Func<SettingsManager, Message, byte[]> Handler;
        public string MessageType { get; }
        public SettingsManager SettingsManager { get; set; }

        public MessageHandler(string messageType, Func<SettingsManager, Message, byte[]> handler, SettingsManager smanager, ICacheProvider<string, Message> cacheProvider = null)
        {
            cache = cacheProvider ?? new SimpleCacheProvider<Message>();
            Handler = handler;
            Checker = new Timer(40000);
            Checker.AutoReset = true;
            Checker.Elapsed += Checker_Elapsed;
            MessageType = messageType;
            SettingsManager = smanager;
        }

        private void Checker_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TODO get 1 or 2 recent blocks to process
        }

        public void Handle(Message message, int retries)
        {
            if (message.MessageType != MessageType)
                throw new InvalidOperationException($"Can not handle a '{message.MessageType}' with a '{MessageType}' handler!");
            if (message.ExpiresDateTime.HasValue && message.ExpiresDateTime < UnixUTCNow - 1)
                //TODO Log expired message.
                return;
            int delay;
            if (message.StartDateTime.HasValue && (delay = (int)(message.StartDateTime - UnixUTCNow)) > 0)
                ScheduleForLater(delay, message);
            else
            {
                try
                {
                    //TODO handle timeout.
                    var result = Handler(SettingsManager, message);
                    if (message.Period > 0)
                        PeriodicRun(message);
                }
                catch (Exception e)
                {
                    //TODO Log exception
                    if (retries > 0)
                        Handle(message, retries - 1);
                }
            }

        }

        private void PeriodicRun(Message message)
        {

        }

        private async Task ScheduleForLater(int delay, Message message)
        {
            //Schedule a thread whithin 5 minutes
            if (delay < 300000)
            {
                await Task.Run(() =>
                {
                    Task.Delay(delay);
                    Handle(message, message.Retries);
                });
            }
            else
            {
                //TODO queue jobs to cache later
            }
        }

    }
}
