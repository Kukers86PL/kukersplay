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
        private string serverip = "";

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

        public void startTCPServer()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 13200);
            server.Start();

            Byte[] bytes = new Byte[256];
            String data = null;

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                data = null;
                NetworkStream stream = client.GetStream();
                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                }
                connectedBox.Invoke(new Action(() => connectedBox.Items.Add(data)));
            }
        }

        public void startUDPServer()
        {
            var Server = new UdpClient(13100);
            var ResponseData = Encoding.ASCII.GetBytes(File.ReadAllText("./ipaddress.txt"));

            while (true)
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
                Thread serverUDP = new Thread(new ThreadStart(startUDPServer));
                serverUDP.Start();
                Thread serverTCP = new Thread(new ThreadStart(startTCPServer));
                serverTCP.Start();

                Client.Close();

                return;
            }

            Client.Close();

            Thread clientTCP = new Thread(new ThreadStart(startTCPClient));
            clientTCP.Start();
        }

        public void startTCPClient()
        {
            while (true)
            {
                if (serverip != "")
                {
                    try
                    {
                        TcpClient client = new TcpClient(serverip.Substring(0, serverip.Length - 2), 13200);

                        Byte[] data = System.Text.Encoding.ASCII.GetBytes(File.ReadAllText("./login.txt"));

                        NetworkStream stream = client.GetStream();

                        stream.Write(data, 0, data.Length);
                        stream.Flush();

                        stream.Close();
                        client.Close();
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
    }
}
