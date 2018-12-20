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
using System.Text.RegularExpressions;

namespace kukersplay
{
    public partial class Form1 : Form
    {
        private IUDPServer m_udpServer = new UDPServer();
        private IUDPClient m_udpClient = new UDPClient();
        private ITCPClient m_tcpClient = new TCPClient();
        private ITCPServer m_tcpServer = new TCPServer();
        private IMessagesManager m_messageManager = new MessagesManager();

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

        private void tcp_client_received_callback(string a_message)
        {
            receive(a_message);
        }

        private void udp_client_received_callback(string a_message)
        {
            receive(a_message);
        }

        private void tcp_server_received_callback(string a_message)
        {
            receive(a_message);
        }

        private void udp_client_timeout_callback()
        {
            m_udpClient.stop();
            m_udpServer.start(m_messageManager.buildServerInfo(File.ReadAllText("./ipaddress.txt")));

            m_tcpServer.start(tcp_server_received_callback);
            connectedBox.Invoke(new Action(() => connectedBox.Items.Add(Regex.Replace(File.ReadAllText("./login.txt"), @"\s+", "") + " - " + Regex.Replace(File.ReadAllText("./ipaddress.txt"), @"\s+", ""))));
        }

        private void allStop()
        {
            m_udpServer.stop();
            m_udpClient.stop();
            m_tcpClient.stop();
            m_tcpServer.stop();
        }

        private void button1_ClickAsync(object sender, EventArgs e)
        {
            allStop();

            File.WriteAllText("./login.txt", loginBox.Text);
            File.WriteAllText("./pass.txt", passBox.Text);
            File.WriteAllText("./server.txt", serverBox.Text);

            Process.Start("powershell", "-ExecutionPolicy Bypass ./kukers86vpn.ps1").WaitForExit();

            if (File.ReadAllText("./result.txt") == "true\r\n")
            {
                m_udpClient.start(udp_client_received_callback, udp_client_timeout_callback);
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
            allStop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string h3path = "";
            try
            {
                h3path = File.ReadAllText("./h3.txt");
            }
            catch (Exception)
            {

            }
            while (true)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = h3path;
                ofd.Multiselect = false;
                ofd.Filter = "Heroes3|Heroes3.exe|All files|*.*";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    h3path = ofd.FileName;
                    break;
                }
                else
                {
                    return;
                }
            }
            File.WriteAllText("./h3.txt", h3path);
        }

        private void receive(string a_message)
        {
            MSG_TYPE msg_type = m_messageManager.parse(a_message);
            switch (msg_type)
            {
                case MSG_TYPE.MSG_CLIENT_INFO_TYPE:
                    connectedBox.Invoke(new Action(() => connectedBox.Items.Add(m_messageManager.getClientLogin() + " - " + m_messageManager.getClientIP())));
                    break;
                case MSG_TYPE.MSG_RESET_INFO_TYPE:
                    m_tcpServer.send(a_message);
                    connectedBox.Invoke(new Action(() => connectedBox.Items.Clear()));
                    connectedBox.Invoke(new Action(() => connectedBox.Items.Add(Regex.Replace(File.ReadAllText("./login.txt"), @"\s+", "") + " - " + Regex.Replace(File.ReadAllText("./ipaddress.txt"), @"\s+", ""))));
                    send(m_messageManager.buildClientInfo(File.ReadAllText("./login.txt"), File.ReadAllText("./ipaddress.txt")));
                    break;
                case MSG_TYPE.MSG_SERVER_INFO_TYPE:
                    m_udpClient.stop();
                    m_tcpClient.start(tcp_client_received_callback, m_messageManager.getServerIP());
                    send(m_messageManager.buildResetInfo());
                    break;
                case MSG_TYPE.MSG_HOST_INFO_TYPE:
                    if (!(File.ReadAllText("./ipaddress.txt").Contains(m_messageManager.getHostIP())))
                    {
                        File.WriteAllText("./hostip.txt", m_messageManager.getHostIP());
                        var startInfo = new ProcessStartInfo();

                        startInfo.WorkingDirectory = Path.GetDirectoryName(File.ReadAllText("./h3.txt"));
                        startInfo.FileName = File.ReadAllText("./h3.txt");

                        Process.Start(startInfo);
                        Process.Start("AutoIt3_x64.exe", "h3newjoin.au3").WaitForExit();
                    }
                    break;
                default:
                    break;
            }
        }

        private void send(string a_message)
        {
            m_tcpServer.send(a_message);
            m_tcpClient.send(a_message);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = Path.GetDirectoryName(File.ReadAllText("./h3.txt"));
            startInfo.FileName = File.ReadAllText("./h3.txt");

            Process.Start(startInfo);
            Process.Start("AutoIt3_x64.exe", "h3newhost.au3").WaitForExit();
            send(m_messageManager.buildHostInfo(File.ReadAllText("./ipaddress.txt"), GAME_TYPE.GAME_H3_TYPE));
        }
    }
}
