using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface
{
    public class MenuItemProgram : MenuItem
    {
        public Action onUse;

        public MenuItemProgram(string name, Action onUse) : base(name)
        {
            this.onUse = onUse;
        }

        public override void Use()
        {
            onUse();
        }
    }
}
