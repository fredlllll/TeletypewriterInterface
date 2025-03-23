using System.Device.Gpio;

namespace TeletypewriterInterface
{
    internal class Program
    {
        public static BitBanger bitBanger = new BitBanger(17);
        public static BitReceiver bitReceiver = new BitReceiver(27);

        public static void WriteOut(string s)
        {
            var data = ITA2Encoder.GetBytes(s);
            foreach (byte c in data)
            {
                Console.Write('.');
                bitBanger.Send(c);
            }
        }

        static void Main()
        {
            Menu currentMenu = new Menu("hauptmenü", new MenuItem[] {
                new ("wetter"),
                new ("ascii art"),
                new ("email lesen"),
                new ("email schreiben"),
                new ("feed")
            });

            while (true)
            {
                WriteOut(currentMenu.ToString());
                while (bitReceiver.bufferedData.Count <= 0)
                {
                    Thread.Sleep(5);
                }
                if(bitReceiver.bufferedData.TryDequeue(out var data))
                {
                    char c = ITA2Encoder.GetChar(data);
                }
            }
        }
    }
}
