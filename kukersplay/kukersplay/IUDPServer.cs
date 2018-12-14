using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kukersplay
{
    interface IUDPServer
    {
        void start(string a_message, int a_port = 13100, int a_interval_ms = 1000);
        void stop();
    }
}
