using System.Device.Gpio;

namespace TeletypewriterInterface
{
    public class BitBanger : IDisposable
    {
        private readonly GpioController gpio = new();
        private readonly int pin;
        private readonly double bitDuration;
        private readonly int bitCount;
        private bool isDisposed = false;

        public BitBanger(int pin, double baud = 50, int bitCount = 5)
        {
            this.pin = pin;
            bitDuration = 1 / baud;
            this.bitCount = bitCount;

            gpio.OpenPin(pin, PinMode.Output);
            gpio.Write(pin, PinValue.High);
        }

        void WriteBit(PinValue bitValue)
        {
            gpio.Write(pin, bitValue);
            Util.Sleep(bitDuration);
        }

        void WriteStopBits()
        {
            gpio.Write(pin, PinValue.High);
            Util.Sleep(bitDuration * 1.5);
        }

        bool GetBit(byte val, int bit)
        {
            return (val & (1 << bit)) != 0;
        }

        public void Send(byte b)
        {
            //start bit
            WriteBit(PinValue.Low);

            for (int i = 0; i < this.bitCount; i++)
            {
                bool bit = GetBit(b, i);
                WriteBit(bit ? PinValue.High : PinValue.Low);
            }

            WriteStopBits();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    gpio.Dispose();
                }
                isDisposed = true;
            }
        }

        ~BitBanger()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
