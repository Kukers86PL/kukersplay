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
        MSG_UNKNOWN_TYPE
    }

    interface IMessagesManager
    {
        MSG_TYPE parse(string a_message);

        string buildServerInfo(string a_host);
        string buildClientInfo(string a_login, string a_clientIP);
        string buildResetInfo();

        string getServerIP();
        string getClientLogin();
        string getClientIP();
    }
}
