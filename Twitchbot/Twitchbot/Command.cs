using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchbot
{
    class Command
    {
        public string Com { get; set; }
        public string Answer { get; set; }

        public Command()
        {
        }
        public override string ToString()
        {
            string s = Com + ":" + Answer;
            return s;
        }
    }
}
