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
    class UDPClient : IUDPClient
    {
        private UdpClient m_client;
        private IPEndPoint m_serverEP;
        private volatile bool m_working;
        private Action<string> m_callback_received = null;
        private Action m_callback_timeout = null;
        private Thread m_clientThread;

        public void start(Action<string> a_callback_received, Action a_callback_timeout, int a_port = 13100, int a_timeout_ms = 10000)
        {
            m_working = true;
            m_client = new UdpClient(a_port);
            m_serverEP = new IPEndPoint(IPAddress.Any, a_port);
            m_client.EnableBroadcast = true;
            m_client.Client.ReceiveTimeout = a_timeout_ms;
            m_callback_received = a_callback_received;
            m_callback_timeout = a_callback_timeout;

            m_clientThread = new Thread(new ThreadStart(workingThread));
            m_clientThread.Start();
        }

        public void stop()
        {
            m_working = false;
            if (m_client != null) m_client.Close();
        }

        private void workingThread()
        {
            while (m_working)
            {
                try
                {
                    var serverResponseData = m_client.Receive(ref m_serverEP);
                    var serverResponse = Encoding.ASCII.GetString(serverResponseData);
                    m_callback_received(serverResponse);
                }
                catch (Exception)
                {
                    // nothing to do
                }
                if (m_working) m_callback_timeout();
            }
        }
    }
}
