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
        private string m_hostIP;
        private GAME_TYPE m_hostGame;

        private static char DELIMITER = ':';
        private static string MSG_SERVER_INFO = "SERVER";
        private static string MSG_CLIENT_INFO = "CLIENT";
        private static string MSG_RESET_INFO = "RESET";
        private static string MSG_HOST_INFO = "HOST";
        private static string GAME_H3_INFO = "H3";

        public string buildClientInfo(string a_login, string a_clientIP)
        {
            return MSG_CLIENT_INFO + DELIMITER + Regex.Replace(a_login, @"\s+", "") + DELIMITER + Regex.Replace(a_clientIP, @"\s+", "");
        }

        public string buildServerInfo(string a_host)
        {
            return MSG_SERVER_INFO + DELIMITER + Regex.Replace(a_host, @"\s+", "");
        }

        public string buildResetInfo()
        {
            return MSG_RESET_INFO;
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
            try
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
                else if (words[0].Contains(MSG_RESET_INFO))
                {
                    return MSG_TYPE.MSG_RESET_INFO_TYPE;
                }
                else if (words[0].Contains(MSG_HOST_INFO))
                {
                    string game = words[1];
                    m_hostIP = words[2];
                    if (game.Contains(GAME_H3_INFO))
                    {
                        m_hostGame = GAME_TYPE.GAME_H3_TYPE;
                    }
                    return MSG_TYPE.MSG_HOST_INFO_TYPE;
                }
            }
            catch (Exception)
            {
                // nothing to do
            }
            return MSG_TYPE.MSG_UNKNOWN_TYPE;
        }

        public string buildHostInfo(string a_host, GAME_TYPE a_game)
        {
            if (a_game == GAME_TYPE.GAME_H3_TYPE)
            {
                return MSG_HOST_INFO + DELIMITER + GAME_H3_INFO + DELIMITER + Regex.Replace(a_host, @"\s+", "");
            }
            return "";
        }

        public string getHostIP()
        {
            return m_hostIP;
        }

        public GAME_TYPE getHostGame()
        {
            return m_hostGame;
        }
    }
}
