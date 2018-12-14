using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kukersplay
{
    interface ITCPServer
    {
        void start(Action<string> a_callback_received, int a_port = 13200);
        void stop();
        void send(string a_message);
    }
}
