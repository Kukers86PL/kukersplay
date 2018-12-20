using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kukersplay
{
    class TCPClient : ITCPClient
    {
        private TcpClient m_client;
        private volatile bool m_running;
        private Action<string> m_callback_received;

        public void send(string a_message)
        {
            try
            {
                NetworkStream stream = m_client.GetStream();
                StreamWriter client_writer = new StreamWriter(stream);
                client_writer.WriteLine(a_message);
                client_writer.Flush();
            }
            catch (Exception)
            {

            }
        }

        public void start(Action<string> a_callback_received, string a_hostname, int a_port = 13200)
        {
            m_client = new TcpClient(a_hostname, a_port);
            m_callback_received = a_callback_received;
            m_running = true;

            Thread clientThread = new Thread(new ThreadStart(workingThreadAsync));
            clientThread.Start();
        }

        public void stop()
        {
            m_running = false;
            if (m_client != null) m_client.Close();
        }

        private async void workingThreadAsync()
        {
            while (m_running)
            {
                try
                {
                    NetworkStream stream = m_client.GetStream();
                    StreamReader client_reader = new StreamReader(stream);
                    string data = await client_reader.ReadLineAsync();
                    if (data != null && data != "")
                    {
                        m_callback_received(data);
                    }
                }
                catch (Exception)
                {
                    // nothing to do
                }
                Thread.Sleep(100);
            }
        }
    }
}
