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
        private byte[] m_message;
        private bool m_working;
        private int m_interval_ms;

        public void start(string a_message = "", int a_port = 13100, int a_interval_ms = 1000)
        {
            m_working = true;
            m_client = new UdpClient(a_port);
            m_message = Encoding.ASCII.GetBytes(a_message);
            m_interval_ms = a_interval_ms;
            Thread server = new Thread(new ThreadStart(workingThread));
            server.Start();
        }

        public void stop()
        {
            m_working = false;
        }

        private void workingThread()
        {
            var clientEp = new IPEndPoint(IPAddress.Broadcast, 13100);
            while (m_working)
            {
                m_client.Send(m_message, m_message.Length, clientEp);
                Thread.Sleep(m_interval_ms);
            }
            m_client.Close();
        }
    }
}
