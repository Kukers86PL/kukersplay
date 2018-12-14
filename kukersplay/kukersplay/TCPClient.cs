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
        private NetworkStream m_client_stream;
        private StreamReader m_client_reader;
        private StreamWriter m_client_writer;
        private volatile bool m_running;
        private Thread m_clientThread;
        private Action<string> m_callback_received;

        public void send(string a_message)
        {
            m_client_writer.WriteLine(a_message);
            m_client_writer.Flush();
        }

        public void start(Action<string> a_callback_received, string a_hostname, int a_port = 13200)
        {
            m_client = new TcpClient(a_hostname, a_port);
            m_client.Client.ReceiveTimeout = 1000;
            m_client_stream = m_client.GetStream();
            m_client_reader = new StreamReader(m_client_stream);
            m_client_writer = new StreamWriter(m_client_stream);
            m_callback_received = a_callback_received;
            m_running = true;

            m_clientThread = new Thread(new ThreadStart(workingThreadAsync));
            m_clientThread.Start();
        }

        public void stop()
        {
            m_running = false;
            if (m_client_writer != null) m_client_writer.Close();
            if (m_client_reader != null) m_client_reader.Close();
            if (m_client_stream != null) m_client_stream.Close();
            if (m_client != null) m_client.Close();
        }

        private void workingThreadAsync()
        {
            while (m_running)
            {
                try
                {
                    string data = m_client_reader.ReadLine();
                    m_callback_received(data);
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
