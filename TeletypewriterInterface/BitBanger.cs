using System.Device.Gpio;

namespace TeletypewriterInterface
{
    class BitBanger : IDisposable
    {
        private static GpioController gpio = new GpioController();
        private bool isDisposed = false;
        private readonly int pin;
        private readonly double bitDuration;
        private readonly int bitCount;

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
            Thread.Sleep(TimeSpan.FromSeconds(bitDuration)); //TODO: better timing mechanism if necessary
        }

        void WriteStopBit()
        {
            gpio.Write(pin, PinValue.High);
            Thread.Sleep(TimeSpan.FromSeconds(bitDuration * 1.5));
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

            WriteStopBit();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {

                }

                gpio.ClosePin(pin);
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
