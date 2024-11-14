using System.Diagnostics;
using System.Threading;
using Unosquare.RaspberryIO.Abstractions;

namespace WakeUpAPI.Gpio.LED
{
    public class LedStripWs2801
    {
        public static int Ws2801SpiChannelFrequency = 1000000;

        private readonly ISpiChannel _spi;
        private readonly byte[] _pixels;
        private readonly int _pixelCount;

        public LedStripWs2801(int pixelCount, ISpiChannel spi)
        {
            _spi = spi;
            _pixelCount = pixelCount;
            _pixels = new byte[pixelCount * 3];
        }

        public static int RgbToColor(byte r, byte g, byte b)
        {
            return r << 16 | g << 8 | b;
        }

        public static byte[] ColorToRgb(int color)
        {
            return new[]
            {
                (byte) (color >> 16),
                (byte) (color >> 8),
                (byte) color
            };
        }

        public int Count()
        {
            return _pixelCount;
        }

        /// <summary>
        /// Push the current pixel values out to the hardware.  Must be called to actually change the pixel colors
        /// </summary>
        public void Render()
        {
            _spi.Write(_pixels);
            Thread.Sleep(2);
        }

        /// <summary>
        /// Clear all the pixels to black/off.  Note you MUST call Render() after
        /// clearing pixels to see the LEDs change!
        /// </summary>
        public void Clear()
        {
            SetPixels(0);
        }

        /// <summary>
        /// Set the specified pixel n to the provided 24-bit RGB color.  Note you
        /// MUST call Render() after setting pixels to see the LEDs change color!
        /// </summary>
        public void SetPixel(int index, int color)
        {
            var rgb = ColorToRgb(color);
            SetPixelRgb(index, rgb[0], rgb[1], rgb[2]);
        }

        /// <summary>
        /// Set the specified pixel n to the provided 8-bit red, green, blue
        /// component values.  Note you MUST call Render() after setting pixels to
        /// see the LEDs change color!
        /// </summary>
        public void SetPixelRgb(int index, byte r, byte g, byte b)
        {
            if (index < 0 || index > _pixelCount)
            {
                Trace.WriteLine("Pixel n outside the count of pixels!");
                return;
            }
            _pixels[index * 3] = (byte)(r & 0xFF);
            _pixels[index * 3 + 1] = (byte)(g & 0xFF);
            _pixels[index * 3 + 2] = (byte)(b & 0xFF);
        }

        /// <summary>
        /// Set all pixels to the provided 24-bit RGB color value.  Note you
        /// MUST call Render() after setting pixels to see the LEDs change!
        /// </summary>
        public void SetPixels(int color)
        {
            var rgb = ColorToRgb(color);
            SetPixelsRgb(rgb[0], rgb[1], rgb[2]);
        }

        /// <summary>
        /// Set all pixels to the provided 8-bit red, green, blue component color
        /// value.  Note you MUST call Render() after setting pixels to see the LEDs
        /// change!
        /// </summary>
        public void SetPixelsRgb(byte r, byte g, byte b)
        {
            for (var i = 0; i < _pixelCount; i++)
            {
                SetPixelRgb(i, r, g, b);
            }
        }

        /// <summary>
        /// Retrieve the 8-bit red, green, blue component color values of the
        /// specified pixel n.  Will return a 3-tuple of red, green, blue data.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte[] GetPixelRgb(int index)
        {
            if (index >= 0 && index < _pixelCount)
                return new[]
                {
                    _pixels[index * 3],
                    _pixels[index * 3 + 1],
                    _pixels[index * 3 + 2]
                };
            Trace.WriteLine("Pixel n outside the count of pixels!");
            return new byte[0];
        }

        /// <summary>
        /// Retrieve the 24-bit RGB color of the specified pixel n.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetPixel(int index)
        {
            var rgb = GetPixelRgb(index);
            return RgbToColor(rgb[0], rgb[1], rgb[2]);
        }
    }
}