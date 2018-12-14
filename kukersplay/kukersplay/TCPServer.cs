using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kukersplay
{
    class TCPServer : ITCPServer
    {
        private volatile bool m_running = true;
        private List<TcpClient> m_clients = new List<TcpClient>();
        private AutoResetEvent m_clientsEvent = new AutoResetEvent(true);
        private TcpListener m_server;
        private Thread m_listeningThread;
        private Thread m_recivingThread;
        private Action<string> m_callback_received;

        public void start(Action<string> a_callback_received, int a_port = 13200)
        {
            m_server = new TcpListener(IPAddress.Any, a_port);
            m_server.Start();
            m_callback_received = a_callback_received;

            m_running = true;

            m_listeningThread = new Thread(new ThreadStart(listeningThreadAsync));
            m_listeningThread.Start();

            m_recivingThread = new Thread(new ThreadStart(recivingThreadAsync));
            m_recivingThread.Start();
        }

        public void stop()
        {
            m_running = false;
            m_server.Stop();
            foreach (TcpClient client in m_clients)
            {
                client.Close();
            }
        }

        public void send(string a_message)
        {
            m_clientsEvent.WaitOne();
            foreach (TcpClient client in m_clients)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    StreamWriter stream_writer = new StreamWriter(stream);
                    stream_writer.WriteLine(a_message);
                    stream_writer.Flush();
                }
                catch (Exception)
                {
                    // nothing to do
                }
            }
            m_clientsEvent.Set();
        }

        private async void listeningThreadAsync()
        {
            while (m_running)
            {
                try
                {
                    TcpClient client = await m_server.AcceptTcpClientAsync();
                    if (client != null)
                    {
                        m_clientsEvent.WaitOne();
                        m_clients.Add(client);
                        m_clientsEvent.Set();
                    }
                }
                catch (Exception)
                {
                    // nothing to do
                }
                Thread.Sleep(100);
            }
        }

        private async void recivingThreadAsync()
        {
            while (m_running)
            {
                m_clientsEvent.WaitOne();
                foreach (TcpClient client in m_clients)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        StreamReader reader = new StreamReader(stream);
                        String data = await reader.ReadLineAsync();
                        if (data != null)
                        {
                            Thread tmpThread = new Thread(callbackThread);
                            tmpThread.Start(data);
                        }
                    }
                    catch (Exception)
                    {
                        // nothing to do
                    }
                }
                m_clientsEvent.Set();
                Thread.Sleep(100);
            }
        }

        private void callbackThread(object data)
        {
            m_callback_received((string)data);
        }
    }
}
