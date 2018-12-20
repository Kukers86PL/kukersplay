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
        private TcpListener m_server;
        private Action<string> m_callback_received;
        private static SemaphoreSlim m_sem = new SemaphoreSlim(1);

        public void start(Action<string> a_callback_received, int a_port = 13200)
        {
            m_server = new TcpListener(IPAddress.Any, a_port);
            m_server.Start();
            m_callback_received = a_callback_received;

            m_running = true;

            Thread listeningThread = new Thread(new ThreadStart(listeningThreadAsync));
            listeningThread.Start();

            Thread recivingThread = new Thread(new ThreadStart(recivingThreadAsync));
            recivingThread.Start();
        }

        public void stop()
        {
            m_running = false;
            if (m_server != null) m_server.Stop();
            m_sem.Wait();
            foreach (TcpClient client in m_clients)
            {
                client.Close();
            }
            m_sem.Release();
        }

        public void send(string a_message)
        {
            m_sem.Wait();
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
            m_sem.Release();
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
                        m_sem.Wait();
                        m_clients.Add(client);
                        m_sem.Release();
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
                m_sem.Wait();
                foreach (TcpClient client in m_clients)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        StreamReader reader = new StreamReader(stream);
                        String data = await reader.ReadLineAsync();
                        if (data != null && data != "")
                        {
                            m_callback_received(data);
                        }
                    }
                    catch (Exception)
                    {
                        // nothing to do
                    }
                }
                m_sem.Release();
                Thread.Sleep(100);
            }
        }
    }
}
