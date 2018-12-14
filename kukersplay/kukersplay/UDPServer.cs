using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kukersplay
{
    class UDPServer : IUDPServer
    {
        private UdpClient m_client;
        private IPEndPoint m_clientEP;
        private byte[] m_message;
        private volatile bool m_working;
        private int m_interval_ms;
        private Thread m_serverThread;

        public void start(string a_message = "", int a_port = 13100, int a_interval_ms = 1000)
        {
            m_working = true;
            m_client = new UdpClient(a_port);
            m_message = Encoding.ASCII.GetBytes(a_message);
            m_interval_ms = a_interval_ms;
            m_clientEP = new IPEndPoint(IPAddress.Broadcast, a_port);

            m_serverThread = new Thread(new ThreadStart(workingThread));
            m_serverThread.Start();
        }

        public void stop()
        {
            m_working = false;
            m_client.Close();
        }

        private void workingThread()
        {
            while (m_working)
            {
                m_client.Send(m_message, m_message.Length, m_clientEP);
                Thread.Sleep(m_interval_ms);
            }
        }
    }
}
