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

        public void startUDPServer()
        {
            var Server = new UdpClient(13100);
            var ResponseData = Encoding.ASCII.GetBytes(File.ReadAllText("./ipaddress.txt"));

            while (running)
            {
                var ClientEp = new IPEndPoint(IPAddress.Broadcast, 13100);
                Server.Send(ResponseData, ResponseData.Length, ClientEp);
                Thread.Sleep(1000);
            }
        }

        public void startUDPClient()
        {
            Thread.Sleep(10000);

            var Client = new UdpClient(13100);
            var ServerEp = new IPEndPoint(IPAddress.Any, 13100);
            Client.EnableBroadcast = true;
            Client.Client.ReceiveTimeout = 10000;

            try
            {
                var ServerResponseData = Client.Receive(ref ServerEp);
                var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                serverip = ServerResponse;
            }
            catch (Exception)
            {
                Client.Close();

                Thread serverUDP = new Thread(new ThreadStart(startUDPServer));
                serverUDP.Start();
                Thread serverTCP = new Thread(new ThreadStart(startTCPServerAsync));
                serverTCP.Start();

                return;
            }

            Client.Close();

            Thread clientTCP = new Thread(new ThreadStart(startTCPClient));
            clientTCP.Start();
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

        private void button1_ClickAsync(object sender, EventArgs e)
        {
            File.WriteAllText("./login.txt", loginBox.Text);
            File.WriteAllText("./pass.txt", passBox.Text);
            File.WriteAllText("./server.txt", serverBox.Text);

            Process.Start("powershell", "-ExecutionPolicy Bypass ./kukers86vpn.ps1").WaitForExit();

            if (File.ReadAllText("./result.txt") == "true\r\n")
            {
                Thread clientUDP = new Thread(new ThreadStart(startUDPClient));
                clientUDP.Start();
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
        }
    }
}
