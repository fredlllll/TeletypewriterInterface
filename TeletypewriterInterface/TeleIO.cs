using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface
{
    public static class TeleIO
    {
        public static readonly BitBanger bitBanger = new BitBanger(17);
        public static readonly BitReceiver bitReceiver = new BitReceiver(27);
        static bool inputIsLettersMode = true;
        static bool outputIsLettersMode = true;

        public static void WriteOut(string s)
        {
            var data = ITA2Encoder.GetBytes(s, outputIsLettersMode);
            foreach (byte c in data)
            {
                if (c == ITA2Encoder.SpecialBytes.letterMode)
                {
                    if (outputIsLettersMode)
                    {
                        continue; //skip if already in letters mode
                    }
                    outputIsLettersMode = true;
                }
                else if (c == ITA2Encoder.SpecialBytes.figuresMode)
                {
                    if (!outputIsLettersMode)
                    {
                        continue; //skip if already in figures mode
                    }
                    outputIsLettersMode = false;
                }
                DebugPrint.WriteDebugByte(c, outputIsLettersMode);
                bitBanger.Send(c);
            }
        }

        public static char ReadNextCharacter()
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
    }
}
