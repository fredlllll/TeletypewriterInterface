using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface
{
    public class MenuItemChangeMenu : MenuItem
    {
        Menu newMenu;
        public MenuItemChangeMenu(string name, Menu newMenu) : base(name)
        {
            this.newMenu = newMenu;
        }

        public override void Use()
        {
            Program.currentMenu = newMenu;
            TeleIO.WriteOut(newMenu.ToString());
        }
    }
}
