﻿using System.Collections.Concurrent;
using System.Device.Gpio;
using System.Diagnostics;

namespace TeletypewriterInterface
{
    public class BitReceiver : IDisposable
    {
        private readonly GpioController gpio = new();
        private readonly int pin;
        private readonly int debugPin = 22;
        private readonly double bitDuration;
        private readonly int bitCount;
        private bool isDisposed = false;
        private bool isReceiving = false;
        private readonly Thread readerThread;
        private readonly AutoResetEvent readerEvent = new(false);

        public readonly ConcurrentQueue<byte> bufferedData = new();

        public BitReceiver(int pin, double baud = 50, int bitCount = 5)
        {
            this.pin = pin;
            bitDuration = 1 / baud;
            this.bitCount = bitCount;

            readerThread = new Thread(RunRead);
            readerThread.Start();
            gpio.OpenPin(pin, PinMode.InputPullDown);
            gpio.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Falling, OnStartBitDetected);

            gpio.OpenPin(debugPin, PinMode.Output);
            gpio.Write(debugPin, PinValue.Low);
        }

        void OnStartBitDetected(object sender, PinValueChangedEventArgs args)
        {
            if (!isReceiving)
            {
                Util.Sleep(bitDuration / 5);
                if (gpio.Read(pin) == PinValue.Low) //check if just a blip or actually low for longer
                {
                    gpio.Write(debugPin, PinValue.High);
                    isReceiving = true;
                    readerEvent.Set();
                    Util.Sleep(bitDuration / 5);
                    gpio.Write(debugPin, PinValue.Low);
                }
            }
        }

        void RunRead()
        {
            while (true)
            {
                readerEvent.WaitOne();
                int receivedByte = ReceiveByte();
                bufferedData.Enqueue((byte)receivedByte);
                isReceiving = false;
            }
        }

        bool ReadBit()
        {
            int highReads = 0;
            for (int i = 0; i < 4; ++i)
            {
                if (gpio.Read(pin) == PinValue.High)
                {
                    highReads++;
                }
                Util.Sleep(bitDuration / 5);
            }
            return highReads > 2;
        }

        int ReceiveByte()
        {
            Util.Sleep(bitDuration); //1/5th into the first bit

            int receivedByte = 0;

            bool debug = true;

            for (int i = 0; i < bitCount; i++)
            {
                gpio.Write(debugPin, debug ? PinValue.High : PinValue.Low);
                debug = !debug;
                if (ReadBit())
                {
                    receivedByte |= 1 << i;
                }
                Util.Sleep(bitDuration / 5);
            }

            // skip stop bits
            Util.Sleep(bitDuration * 0.8); //as we trigger on falling edge, we can exit half a bit early to be sure to catch the next startbit
            gpio.Write(debugPin, PinValue.Low);

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
