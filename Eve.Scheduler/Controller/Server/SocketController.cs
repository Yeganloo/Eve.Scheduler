using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Eve.Scheduler.Controller.Server
{
    public class SocketController : IDisposable
    {
        private SimpleLogger _Logger;
        private IList<AsyncTcpServer> _Servers;
        private Dictionary<string, SocketSettings> _SocketsSettings;
        private readonly Dictionary<string, MessageHandler> _Handlers;

        public SocketController(SimpleLogger logger, Dictionary<string, MessageHandler> handlers)
        {
            _Logger = logger;
            _Servers = new List<AsyncTcpServer>();
            _SocketsSettings = new Dictionary<string, SocketSettings>();
            _Handlers = handlers;
        }

        public void Add(SocketSettings settings)
        {
            var server = new AsyncTcpServer(IPEndPoint.Parse(settings.Address), settings.Name, _Logger);
            _SocketsSettings.Add(settings.Name, settings);
            _Servers.Add(server);
            server.OnDataReceived += Server_OnDataReceived;
            server.StartAsync();
        }

        private void Server_OnDataReceived(object sender, AsyncTcpServer.DataReceivedEventArgs e)
        {
            try
            {
                var message = MessagePackSerializer.Deserialize<Message>(e.NetStream, ContractlessStandardResolver.Options);
                ((TcpClient)sender).Close();
                var settings = _SocketsSettings[e.Receiver];
                if (settings.WhiteList.Contains(message.MessageType))
                {
                    _Handlers[message.MessageType].Handle(message);
                }
                else
                {
                    _Logger.Log(e.Receiver, new { message = "Failed To handle message!", Packet = message }, LogLevels.Error);
                }
            }
            catch (Exception ex)
            {
                _Logger.Log(e.Receiver, ex.ToLog("Failed To Receive Message!"), LogLevels.Error);
            }
        }

        public void Dispose()
        {
            foreach (var server in _Servers)
            {
                server.Dispose();
            }
        }
    }
}
