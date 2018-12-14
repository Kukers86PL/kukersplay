using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kukersplay
{
    interface ITCPClient
    {
        void start(Action<string> a_callback_received, string a_hostname, int a_port = 13200);
        void stop();
        void send(string a_message);
    }
}
