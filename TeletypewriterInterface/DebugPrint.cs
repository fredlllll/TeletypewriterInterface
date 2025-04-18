namespace TeletypewriterInterface
{
    public static class DebugPrint
    {
        public static bool debugPrintEnabled = true;

        public static void WriteDebugByte(byte b, bool outputIsLettersMode)
        {
            if (debugPrintEnabled)
            {
                char c = ITA2Encoder.GetChar(b, outputIsLettersMode);
                WriteDebugChar(c);
            }
        }

        public static void WriteDebugChar(char c)
        {
            if (!debugPrintEnabled)
            {
                return;
            }
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
    }
}
