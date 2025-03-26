using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface
{
    public class Menu
    {
        string name;
        MenuItem[] items;

        public Menu(string name, IEnumerable<MenuItem> items)
        {
            this.name = name;
            this.items = items.ToArray();
        }

        public override string ToString()
        {
            string s = $"--- {name} ---\r\n";
            for (int i = 0; i < items.Length; ++i)
            {
                var item = items[i];
                s += $"{i + 1}) {item.name}\r\n";
            }
            s += ITA2Encoder.SpecialChars.bell;
            return s;
        }

        public void WaitOnInput()
        {
            while (true)
            {
                char c = TeleIO.ReadNextCharacter();
                TeleIO.WriteDebugChar(c);
                if ("123456789".Contains(c))
                {
                    try
                    {
                        Use(c - '1' + 1);
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("error using menu");
                    }
                }
                else if (c == '?')
                {
                    TeleIO.WriteOut(this.ToString());
                    TeleIO.bitReceiver.bufferedData.Clear();
                }
            }
        }

        public void Use(int number)
        {
            Console.WriteLine("using menu item " + number);
            var item = items[number - 1];
            item.Use();
        }
    }
}
