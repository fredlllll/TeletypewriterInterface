using System.Collections.Concurrent;
using System.Device.Gpio;

namespace TeletypewriterInterface
{
    public class BitReceiver : IDisposable
    {
        private readonly GpioController gpio = new();
        private readonly int pin;
        private readonly double bitDuration;
        private readonly int bitCount;
        private bool isDisposed = false;
        private bool isReceiving = false;

        public readonly ConcurrentQueue<byte> bufferedData = new();

        public BitReceiver(int pin, double baud = 50, int bitCount = 5)
        {
            this.pin = pin;
            bitDuration = 1 / baud;
            this.bitCount = bitCount;

            gpio.OpenPin(pin, PinMode.InputPullDown);
            gpio.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Falling, OnStartBitDetected);
        }

        void OnStartBitDetected(object sender, PinValueChangedEventArgs args)
        {
            if (!isReceiving)
            {
                isReceiving = true;

                int receivedByte = ReceiveByte();
                bufferedData.Enqueue((byte)receivedByte);
                isReceiving = false;
            }
        }

        bool ReadBit()
        {
            return gpio.Read(pin) == PinValue.High;
        }

        int ReceiveByte()
        {
            Util.Sleep(bitDuration * 1.5); // wait start bit + 50% of bit duration to be close to the edge

            int receivedByte = 0;

            for (int i = 0; i < bitCount; i++)
            {
                receivedByte |= (ReadBit() ? 1 : 0) << i;
                Util.Sleep(bitDuration);
            }

            // skip stop bits
            Util.Sleep(bitDuration * 1.5); //as we trigger on falling edge, we can exit half a bit early to be sure to catch the next startbit

            return receivedByte;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                gpio.UnregisterCallbackForPinValueChangedEvent(pin, OnStartBitDetected);
                if (disposing)
                {
                    gpio.Dispose();
                }
                isDisposed = true;
            }
        }

        ~BitReceiver()
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
