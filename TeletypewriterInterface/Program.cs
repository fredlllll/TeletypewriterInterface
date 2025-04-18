namespace TeletypewriterInterface
{
    internal class Program
    {
        //public static readonly Menu emailReadMenu = null;
        //public static readonly Menu emailWriteMenu = null;

        public static readonly Menu mainMenu = new Menu("hauptmenue", new MenuItem[] {
                new MenuItemProgram("wetter",Programs.Weather.Run),
                new MenuItemProgram("text art",Programs.TextArt.Run),
                //new MenuItemChangeMenu("email lesen",emailReadMenu),
                //new MenuItemChangeMenu("email schreiben",emailWriteMenu),
                new MenuItemProgram("echo",Programs.Echo.Run)
            });

        public static Menu currentMenu = mainMenu;

        static void ProgramMain()
        {
            //return slide + ring bell
            TeleIO.WriteOut(ITA2Encoder.SpecialChars.figuresMode + "\r\r" + ITA2Encoder.SpecialChars.bell + ITA2Encoder.SpecialChars.letterMode);
            while (true)
            {
                currentMenu.WaitOnInput();
            }
        }

        static void ProgramInput()
        {
            //program to only read input and print on console
            while (true)
            {
                char c = TeleIO.ReadNextCharacter();
                DebugPrint.WriteDebugChar(c);
            }
        }

        static void ProgramOutput()
        {
            //program to just output all characters continously
            TeleIO.bitBanger.Send(ITA2Encoder.SpecialBytes.letterMode);
            while (true)
            {
                TeleIO.WriteOut("abcdefghijklmnopqrstuvwxyz -,:?.'()/+=0123456789" + ITA2Encoder.SpecialChars.bell + "\r\n");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ProgramMain();
            }
            else
            {
                switch (args[0])
                {
                    case "input":
                        ProgramInput();
                        break;
                    case "output":
                        ProgramOutput();
                        break;
                }
            }
        }
    }
}
