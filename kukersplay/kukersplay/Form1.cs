﻿using System;
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

        private void received_callback_3(string a_message)
        {
            MSG_TYPE msg_type = m_messageManager.parse(a_message);
            switch (msg_type)
            {
                case MSG_TYPE.MSG_CLIENT_INFO_TYPE:
                    connectedBox.Invoke(new Action(() => connectedBox.Items.Add(m_messageManager.getClientLogin() + " - " + m_messageManager.getClientIP())));
                    break;
                default:
                    // nothing to do
                    break;
            }
        }

        private void received_callback(string a_message)
        {
            m_udpClient.stop();
            MSG_TYPE msg_type = m_messageManager.parse(a_message);
            switch (msg_type)
            {
                case MSG_TYPE.MSG_SERVER_INFO_TYPE:
                    m_tcpClient.start(received_callback_3, m_messageManager.getServerIP());
                    m_tcpClient.send(m_messageManager.buildClientInfo(File.ReadAllText("./login.txt"), File.ReadAllText("./ipaddress.txt")));
                    break;
                default:
                    // nothing to do
                    break;
            }
        }

        private void received_callback_2(string a_message)
        {
            MSG_TYPE msg_type = m_messageManager.parse(a_message);
            switch (msg_type)
            {
                case MSG_TYPE.MSG_CLIENT_INFO_TYPE:
                    connectedBox.Invoke(new Action(() => connectedBox.Items.Add(m_messageManager.getClientLogin() + " - " + m_messageManager.getClientIP())));
                    break;
                default:
                    break;
            }
            m_tcpServer.send(m_messageManager.buildClientInfo(File.ReadAllText("./login.txt"), File.ReadAllText("./ipaddress.txt")));
        }

        private void timeout_callback()
        {
            m_udpClient.stop();
            m_udpServer.start(m_messageManager.buildServerInfo(File.ReadAllText("./ipaddress.txt")));

            m_tcpServer.start(received_callback_2);
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
            m_udpServer.stop();
            m_udpClient.stop();
            m_tcpClient.stop();
            m_tcpServer.stop();
        }
    }
}
