using System.Device.Gpio;

namespace TeletypewriterInterface
{
    internal class Program
    {
        public static readonly BitBanger bitBanger = new BitBanger(17);
        public static readonly BitReceiver bitReceiver = new BitReceiver(27);
        public static readonly Menu mainMenu = new Menu("hauptmenue", new MenuItem[] {
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
                char visibleChar = ITA2Encoder.GetChar(c, isLettersMode);
                WriteDebugChar(visibleChar);
                bitBanger.Send(c);
            }
        }

        static void WriteDebugChar(char c)
        {
            switch (c)
            {
                case ITA2Encoder.SpecialChars.lineFeed:
                    Console.WriteLine("\\n");
                    break;
                case ITA2Encoder.SpecialChars.carriageReturn:
                    Console.Write("\\r");
                    break;
                case ITA2Encoder.SpecialChars.letterMode:
                    Console.Write("\\x0f");
                    break;
                case ITA2Encoder.SpecialChars.figuresMode:
                    Console.Write("\\x0e");
                    break;
                case ITA2Encoder.SpecialChars.whoAreYou:
                    Console.Write("\\x05");
                    break;
                case ITA2Encoder.SpecialChars.bell:
                    Console.Write("\\x07");
                    break;
                default:
                    Console.Write(c);
                    break;
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
                    WriteDebugChar(c);
                    if ("123456789".Contains(c))
                    {
                        try
                        {
                            currentMenu.Use(c - '1' + 1);
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("error using menu");
                        }
                    }
                }
            }
        }
    }
}
