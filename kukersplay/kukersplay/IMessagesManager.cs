using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kukersplay
{
    enum MSG_TYPE
    {
        MSG_SERVER_INFO_TYPE = 0,
        MSG_CLIENT_INFO_TYPE,
        MSG_RESET_INFO_TYPE,
        MSG_HOST_INFO_TYPE,
        MSG_UNKNOWN_TYPE
    }

    enum GAME_TYPE
    {
        GAME_H3_NEW_TYPE = 0,
        GAME_H3_LOAD_TYPE
    }

    interface IMessagesManager
    {
        MSG_TYPE parse(string a_message);

        string buildServerInfo(string a_host);
        string buildClientInfo(string a_login, string a_clientIP);
        string buildResetInfo();
        string buildHostInfo(string a_host, GAME_TYPE a_game);

        string getServerIP();
        string getClientLogin();
        string getClientIP();
        string getHostIP();
        GAME_TYPE getHostGame();
    }
}
