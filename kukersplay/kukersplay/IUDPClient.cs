using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kukersplay
{
    interface IUDPClient
    {
        void start(Action<string> a_callback_received, Action a_callback_timeout, int a_port = 13100, int a_timeout_ms = 10000);
        void stop();
    }
}
