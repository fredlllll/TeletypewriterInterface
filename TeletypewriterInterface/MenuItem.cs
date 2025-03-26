using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface
{
    public abstract class MenuItem
    {
        public string name;

        public MenuItem(string name)
        {
            this.name = name;
        }

        public abstract void Use();
    }
}
