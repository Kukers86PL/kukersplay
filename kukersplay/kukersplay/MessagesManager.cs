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
        private string[] m_clients;

        private static char DELIMITER = ':';
        private static char MSG_CLIENT_INFO_LIST_DELIMITER = '-';
        private static string MSG_SERVER_INFO = "SERVER";
        private static string MSG_CLIENT_INFO = "CLIENT";
        private static string MSG_CLIENT_INFO_LIST = "CLIENTS";

        public string buildClientInfo(string a_login, string a_clientIP)
        {
            return MSG_CLIENT_INFO + DELIMITER + Regex.Replace(a_login, @"\s+", "") + DELIMITER + Regex.Replace(a_clientIP, @"\s+", "");
        }

        public string buildServerInfo(string a_host)
        {
            return MSG_SERVER_INFO + DELIMITER + Regex.Replace(a_host, @"\s+", "");
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
            string[] words = Regex.Replace(a_message, @"\s+", "").Split(DELIMITER);
            if (words[0].Contains(MSG_SERVER_INFO))
            {   
                m_serverIP = words[1];
                return MSG_TYPE.MSG_SERVER_INFO_TYPE;
            }
            else if (words[0].Contains(MSG_CLIENT_INFO))
            {
                m_clientLogin = words[1];
                m_clientIP = words[2];
                return MSG_TYPE.MSG_CLIENT_INFO_TYPE;
            }
            else if (words[0].Contains(MSG_CLIENT_INFO_LIST))
            {
                List<string> list = new List<string>();
                for (int i = 1; i < words.Length; i++)
                {
                    list.Add(words[i]);
                }
                m_clients = list.ToArray();
                return MSG_TYPE.MSG_CLIENT_INFO_TYPE;
            }
            return MSG_TYPE.MSG_UNKNOWN_TYPE;
        }

        public string buildClientInfoList(string[] a_login, string[] a_clientIP)
        {
            string result = MSG_CLIENT_INFO_LIST + DELIMITER;
            for (int i = 0; i < a_login.Length; i++)
            {
                result += Regex.Replace(a_login[i], @"\s+", "") + MSG_CLIENT_INFO_LIST_DELIMITER + Regex.Replace(a_clientIP[i], @"\s+", "") + DELIMITER;
            }
            return result;
        }

        public string[] getClientInfoList()
        {
            return m_clients;
        }
    }
}
