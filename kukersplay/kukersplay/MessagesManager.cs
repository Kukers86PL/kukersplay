using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kukersplay
{
    class MessagesManager : IMessagesManager
    {
        private string m_serverIP;
        private string m_clientIP;
        private string m_clientLogin;

        private static string DELIMITER = ":";
        private static string MSG_SERVER_INFO = "SERVER" + DELIMITER;
        private static string MSG_CLIENT_INFO = "CLIENT" + DELIMITER;

        public string buildClientInfo(string a_login, string a_clientIP)
        {
            return MSG_CLIENT_INFO + a_login + DELIMITER + a_clientIP;
        }

        public string buildServerInfo(string a_host)
        {
            return MSG_SERVER_INFO + a_host;
        }

        public string getClientIP()
        {
            return m_clientIP;
        }

        public string getClientLogin()
        {
            return m_clientLogin;
        }

        public string getServerIP()
        {
            return m_serverIP;
        }

        public MSG_TYPE parse(string a_message)
        {
            if (a_message.Substring(0, MSG_SERVER_INFO.Length) == MSG_SERVER_INFO)
            {
                m_serverIP = a_message.Substring(MSG_SERVER_INFO.Length, a_message.Length - MSG_SERVER_INFO.Length);
                m_serverIP = Regex.Replace(m_serverIP, @"\s+", "");
                return MSG_TYPE.MSG_SERVER_INFO_TYPE;
            }
            else if (a_message.Substring(0, MSG_CLIENT_INFO.Length) == MSG_CLIENT_INFO)
            {
                int index1 = a_message.IndexOf(DELIMITER, MSG_CLIENT_INFO.Length);
                m_clientLogin = a_message.Substring(MSG_CLIENT_INFO.Length, index1 - MSG_CLIENT_INFO.Length);
                m_clientIP = a_message.Substring(index1 + 1, a_message.Length - index1 - 1);
                return MSG_TYPE.MSG_CLIENT_INFO_TYPE;
            }
            return MSG_TYPE.MSG_UNKNOWN_TYPE;
        }
    }
}
