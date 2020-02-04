using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Eve.Scheduler.Controller.Server
{
    public class AsyncTcpServer : IDisposable
    {
        private readonly TcpListener _listener;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private SimpleLogger _Logger;
        public string Name { get; private set; }

        public struct DataReceivedEventArgs
        {
            public readonly NetworkStream NetStream;
            public readonly IPAddress IP;
            public readonly string Receiver;

            public DataReceivedEventArgs(string receiver, NetworkStream netstream, IPAddress ip)
            {
                NetStream = netstream;
                IP = ip;
                Receiver = receiver;
            }
        }

        public struct ClientAcceptEventArgs
        {
            public readonly TcpClient Client;
            public readonly string Receiver;

            public ClientAcceptEventArgs(string receiver, TcpClient client)
            {
                Client = client;
                Receiver = receiver;
            }
        }

        public event EventHandler<DataReceivedEventArgs> OnDataReceived;
        public event EventHandler<ClientAcceptEventArgs> OnClientAccept;

        public AsyncTcpServer(IPEndPoint address, string name, SimpleLogger logger)
        {
            _listener = new TcpListener(address);
            _Logger = logger;
            Name = name;
            _Logger.CreateLogger(Name);
        }

        public bool Listening { get; private set; }

        public async Task StartAsync(CancellationToken? token = null)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token ?? new CancellationToken());
            _token = _tokenSource.Token;
            _listener.Start();
            Listening = true;

            try
            {
                await Task.Run(async () =>
                {
                    while (!_token.IsCancellationRequested)
                    {
                        try
                        {
                            var result = await _listener.AcceptTcpClientAsync();
                            ListenToClientAsync(result, _token);
                            OnClientAccept?.Invoke(this, new ClientAcceptEventArgs(Name, result));
                        }
                        catch (Exception e)
                        {
                            _Logger.Log(Name, e.ToLog("Failed to accept client!"), LogLevels.Fatal);
                        }
                    }
                }, _token);
            }
            catch (Exception e)
            {
                _Logger.Log(Name, e.ToLog("Unexpected exit from listenner!"), LogLevels.Fatal);
            }
            finally
            {
                _listener.Stop();
                Listening = false;
            }
        }

        private async void ListenToClientAsync(TcpClient client, CancellationToken token)
        {
            await Task.Run(() =>
            {
                try
                {
                    while (client.Connected && !_token.IsCancellationRequested)
                    {
                        OnDataReceived?.Invoke(client, new DataReceivedEventArgs(Name, client.GetStream(), ((IPEndPoint)client.Client.RemoteEndPoint).Address));
                    }
                }
                catch (Exception e)
                {
                    _Logger.Log(Name, e.ToLog("Client closed unexpectedly!"), LogLevels.Fatal);
                }
                finally
                {
                    try
                    {
                        client.Close();
                    }
                    catch (Exception e)
                    {
                        _Logger.Log(Name, e.ToLog("Client closed with error!"), LogLevels.Fatal);
                    }
                }
            });
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}