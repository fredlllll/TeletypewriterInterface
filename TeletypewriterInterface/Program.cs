using System.Device.Gpio;

namespace TeletypewriterInterface
{
    internal class Program
    {
        public static readonly BitBanger bitBanger = new BitBanger(17);
        public static readonly BitReceiver bitReceiver = new BitReceiver(27);
        public static readonly Menu mainMenu = new Menu("hauptmenü", new MenuItem[] {
                new MenuItemProgram("wetter",Programs.Weather.Run),
                new MenuItemProgram("text art",Programs.TextArt.Run),
                new ("email lesen"),
                new ("email schreiben"),
                new ("feed")
            });

        static bool inputIsLettersMode = true;
        public static Menu currentMenu = mainMenu;

        public static void WriteOut(string s)
        {
            var data = ITA2Encoder.GetBytes(s);
            bool isLettersMode = true;
            foreach (byte c in data)
            {
                if (c == ITA2Encoder.SpecialBytes.letterMode)
                {
                    isLettersMode = true;
                }
                else if (c == ITA2Encoder.SpecialBytes.figuresMode)
                {
                    isLettersMode = false;
                }
                Console.Write(ITA2Encoder.GetChar(c, isLettersMode));
                bitBanger.Send(c);
            }
        }

        static char ReadNextCharacter()
        {
            while (true)
            {
                while (bitReceiver.bufferedData.Count <= 0)
                {
                    Thread.Sleep(5); //wait for input
                }
                if (bitReceiver.bufferedData.TryDequeue(out var data))
                {
                    char c = ITA2Encoder.GetChar(data, inputIsLettersMode);
                    if (c == ITA2Encoder.SpecialChars.figuresMode)
                    {
                        inputIsLettersMode = false;
                        continue;
                    }
                    if (c == ITA2Encoder.SpecialChars.letterMode)
                    {
                        inputIsLettersMode = true;
                        continue;
                    }
                    return c;
                }
            }
        }

        static void Main()
        {

            while (true)
            {
                WriteOut(currentMenu.ToString());
                bitReceiver.bufferedData.Clear();

                while (true)
                {
                    char c = ReadNextCharacter();
                    if ("123456789".Contains(c))
                    {
                        currentMenu.Use(c - '1' + 1);
                        break;
                    }
                }
            }
        }
    }
}
