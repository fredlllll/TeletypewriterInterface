using System.Device.Gpio;

namespace TeletypewriterInterface
{
    internal class Program
    {
        static void Main()
        {
            var bitBanger = new BitBanger(17);

            string text = "HELLO 123\r\n" + ITA2Encoder.bell;
            var data = ITA2Encoder.GetBytes(text);

            foreach (byte c in data)
            {
                Console.Write('.');
                bitBanger.Send(c);
            }
        }
    }
}
