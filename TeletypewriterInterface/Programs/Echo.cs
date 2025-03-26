using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface.Programs
{
    public static class Echo
    {
        public static void Run()
        {
            int newlineCount = 0;
            while (true)
            {
                char c = TeleIO.ReadNextCharacter();
                TeleIO.WriteOut(c.ToString());
                if (c == '\n')
                {
                    newlineCount++;
                }
                else
                {
                    newlineCount = 0;
                }
                if (newlineCount > 1)
                {
                    break;
                }
            }
        }
    }
}
