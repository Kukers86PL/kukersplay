using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace kukersplay
{
    public partial class Form1 : Form
    {
        private static string serverip = "";
        private static volatile bool running = true;
        private static List<TcpClient> clients = new List<TcpClient>();
        private static AutoResetEvent clientsEvent = new AutoResetEvent(true);
        private UDPServer m_udpServer = new UDPServer();
        private UDPClient m_udpClient = new UDPClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                loginBox.Text = File.ReadAllText("./login.txt");
                passBox.Text = File.ReadAllText("./pass.txt");
                serverBox.Text = File.ReadAllText("./server.txt");
            }
            catch (System.IO.IOException)
            {
                // do nothing
            }
        }

        public async void startTCPClientsRead()
        {
            while (running)
            {
                clientsEvent.WaitOne();
                List<TcpClient> toRemove = new List<TcpClient>();
                foreach (TcpClient client in clients)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        StreamReader reader = new StreamReader(stream);
                        String data = await reader.ReadLineAsync();
                        if (data != null && data != "") connectedBox.Invoke(new Action(() => connectedBox.Items.Add(data)));
                    }
                    catch (Exception)
                    {
                        toRemove.Add(client);
                    }
                }
                foreach (TcpClient client in toRemove)
                {
                    clients.Remove(client);
                }
                clientsEvent.Set();
                Thread.Sleep(100);
            }
        }

        public async void startTCPServerAsync()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 13200);
            server.Start();

            Thread clientsRead = new Thread(new ThreadStart(startTCPClientsRead));
            clientsRead.Start();

            while (running)
            {
                try
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    if (client != null)
                    {
                        clientsEvent.WaitOne();
                        clients.Add(client);
                        clientsEvent.Set();
                    }
                }
                catch (Exception)
                {
                    // nothing to do
                }
                Thread.Sleep(100);
            }
        }

        public void startTCPClient()
        {
            while (running)
            {
                if (serverip != "")
                {
                    try
                    {
                        TcpClient client = new TcpClient(serverip.Substring(0, serverip.Length - 2), 13200);

                        Byte[] data = System.Text.Encoding.ASCII.GetBytes(File.ReadAllText("./login.txt"));

                        NetworkStream stream = client.GetStream();
                        StreamWriter writer = new StreamWriter(stream);

                        writer.WriteLine(File.ReadAllText("./login.txt"));
                        writer.Flush();

                        return;
                    }
                    catch (Exception)
                    {
                        // nothing to do
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void received_callback(string a_message)
        {
            serverip = a_message;

            Thread clientTCP = new Thread(new ThreadStart(startTCPClient));
            clientTCP.Start();
        }

        private void timeout_callback()
        {
            m_udpClient.stop();
            Thread.Sleep(1000);
            m_udpServer.start(File.ReadAllText("./ipaddress.txt"));

            Thread serverTCP = new Thread(new ThreadStart(startTCPServerAsync));
            serverTCP.Start();
        }

        private void button1_ClickAsync(object sender, EventArgs e)
        {
            File.WriteAllText("./login.txt", loginBox.Text);
            File.WriteAllText("./pass.txt", passBox.Text);
            File.WriteAllText("./server.txt", serverBox.Text);

            Process.Start("powershell", "-ExecutionPolicy Bypass ./kukers86vpn.ps1").WaitForExit();

            if (File.ReadAllText("./result.txt") == "true\r\n")
            {
                m_udpClient.start(received_callback, timeout_callback);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            running = false;
            m_udpServer.stop();
            m_udpClient.stop();
        }
    }
}
