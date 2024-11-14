using System;
using System.Threading;

namespace WakeUpAPI.Gpio.LED
{
	public class LedPrograms
	{
		private static int ColorWheel(int pos)
		{
			if (pos < 85)
			{
				return LedStripWs2801.RgbToColor((byte)(pos * 3), (byte)(255 - pos * 3), 0);
			}
			else if (pos < 170)
			{
				pos -= 85;
				return LedStripWs2801.RgbToColor((byte)(255 - pos * 3), 0, (byte)(pos * 3));
			}
			else
			{
				pos -= 170;
				return LedStripWs2801.RgbToColor(0, (byte)(pos * 3), (byte)(255 - pos * 3));
			}
		}

		public static void RainbowDieDown(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			const int seconds = 5;
			const int iterator = 25;

			led.Clear();
			led.Render();

			var rgb = new[]
			{
				(byte)(int) (new Random().NextDouble() * 255),
				(byte)(int) (new Random().NextDouble() * 255),
				(byte)(int) (new Random().NextDouble() * 255)
			};

			var pos = (int)Math.Floor(new Random().NextDouble() * led.Count());
			const int step = 255 / iterator;

			for (var i = 0; i < iterator; i++)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				led.SetPixel(pos, LedStripWs2801.RgbToColor(rgb[0], rgb[1], rgb[2]));
				led.Render();

				rgb = new[]
				{
					(byte) Math.Max(0, rgb[0] - step),
					(byte) Math.Max(0, rgb[1] - step),
					(byte) Math.Max(0, rgb[2] - step)
				};
				Thread.Sleep(seconds * 1000 / iterator);
			}
		}

		public static void Glitter(LedStripWs2801 led, CancellationToken cancellationToken, int color)
		{
			var pixelCount = led.Count();

			for (var i = 0; i < pixelCount; i++)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				var rdm = new[] { 0, 0, 0, 0 };
				var colors = new[] { 0, 0, 0, 0 };

				for (var j = 0; j < 3; j++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					rdm[j] = (byte)(int)(new Random().NextDouble() * (pixelCount - 1));

					while (led.GetPixel(rdm[j]) == color)
					{
						if (cancellationToken.IsCancellationRequested)
							return;

						rdm[j]++;

						if (rdm[j] >= pixelCount)
							rdm[j] = 0;
					}

					colors[j] = led.GetPixel(rdm[j]);
				}

				for (var j = 0; j < 3; j++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					led.SetPixel(rdm[j], color);
				}
				led.Render();
				Thread.Sleep(50);

				for (var j = 1; j < 3; j++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					if (rdm[0] >= rdm[j])
					{
						led.SetPixel(rdm[j], colors[j]);
					}
				}
				led.Render();
				led.Clear();
				led.Render();
			}
			Thread.Sleep(100);
		}

		public static void RainbowCycleSuccessive(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			var wait = 100;

			for (var i = 0; i < led.Count(); i++)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				// tricky math! we use each pixel as a fraction of the full 96-color wheel
				// (thats the i / strip.numPixels() part)
				// Then add in j which makes the colors go around per pixel
				// the % 96 is to make the wheel cycle around
				var color = ColorWheel(i * (int)Math.Floor((double)256 / led.Count()) % 256);
				led.SetPixel(i, color);
				led.Render();

				if (wait > 0)
					Thread.Sleep(wait);
			}
		}

		public static void RainbowCycle(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			var wait = 50;

			for (var j = 0; j < 256; j++)
			{
				for (var i = 0; i < led.Count(); i++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					var color = ColorWheel((i * (int)Math.Floor((double)256 / led.Count()) + j) % 256);
					led.SetPixel(i, color);
				}
				led.Render();

				if (wait > 0)
					Thread.Sleep(wait);
			}
		}

		public static void RainbowColors(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			var wait = 50;

			for (var j = 0; j < 256; j++)
			{
				for (var i = 0; i < led.Count(); i++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					var color = ColorWheel(((int)Math.Floor((double)256 / led.Count()) + j) % 256);
					led.SetPixel(i, color);
				}
				led.Render();

				if (wait > 0)
					Thread.Sleep(wait);
			}
		}

		public static void BlinkColor(LedStripWs2801 led, CancellationToken cancellationToken, int color = 0)
		{
			var wait = 500;
			var randomColor = color == 0;

			if (cancellationToken.IsCancellationRequested)
				return;

			led.Clear();
			led.Render();

			if (randomColor)
			{
				var random = new Random();
				var randomRgb = random.Next(0, 2);
				var rgb = new byte[] { 0, 0, 0 };
				rgb[randomRgb] = (byte)(random.NextDouble() * 255);
				color = LedStripWs2801.RgbToColor(rgb[0], rgb[1], rgb[2]);
			}

			for (var i = 0; i < 2; i++)
			{
				for (var k = 0; k < led.Count(); k++)
				{
					led.SetPixel(k, color);
				}
				led.Render();
				Thread.Sleep(80);
				led.Clear();
				led.Render();
				Thread.Sleep(80);
			}

			if (wait > 0)
				Thread.Sleep(wait);
		}

		public static void BrightnessDecrease(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			var wait = 10;
			var step = 1;

			for (var j = 0; j < 256 / step; j++)
			{
				for (var i = 0; i < led.Count(); i++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					var rgb = led.GetPixelRgb(i);
					rgb[0] = (byte)Math.Max(0, rgb[0] - step);
					rgb[1] = (byte)Math.Max(0, rgb[1] - step);
					rgb[2] = (byte)Math.Max(0, rgb[2] - step);
					led.SetPixelRgb(i, rgb[0], rgb[1], rgb[2]);
				}
				led.Render();

				if (wait > 0)
					Thread.Sleep(wait);
			}
		}

		public static void AppearFromBack(LedStripWs2801 led, CancellationToken cancellationToken, int color = 0)
		{
			if (color == 0)
				color = LedStripWs2801.RgbToColor(255, 0, 0);

			for (var i = 0; i < led.Count(); i++)
			{
				for (var j = led.Count() - 1; j > 0; j--)
				{
					led.Clear();

					for (var k = 0; k < i; k++)
					{
						if (cancellationToken.IsCancellationRequested)
							return;

						led.SetPixel(k, color);
					}
					led.SetPixel(j, color);
					led.Render();
				}
			}
		}

		public static void AllOn(LedStripWs2801 led, CancellationToken cancellationToken, int color = 0)
		{
			if (color == 0)
				color = 0x999999;

			if (cancellationToken.IsCancellationRequested)
				return;

			led.SetPixels(color);
			led.Render();
		}

		public static void AllOff(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
				return;

			led.Clear();
			led.Render();
		}

		public static void SoftOn(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			for (var b = 0; b < 99; b++)
			{
				var x = b * 65536 + b * 256 + b;
				for (var i = 0; i < led.Count(); i++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					led.SetPixel(i, x);
				}
				led.Render();
				Thread.Sleep(1000);
			}
		}

		public static void SoftOff(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			for (var b = 99; b > -1; b--)
			{
				var x = b * 65536 + b * 256 + b;
				for (var i = 0; i < led.Count(); i++)
				{
					if (cancellationToken.IsCancellationRequested)
						return;

					led.SetPixel(i, x);
				}
				led.Render();
				Thread.Sleep(2000);
			}
		}

		public static void SunriseSimulation(LedStripWs2801 led, CancellationToken cancellationToken)
		{
			const int waitInterval = 200;
			byte r = 0;
			byte g = 0;
			byte b = 0;

			// Gradual red light increase
			for (var red = 0; red < 256; red++)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				r = (byte)red;
				led.SetPixelsRgb(r, g, b);
				led.Render();
				Thread.Sleep(waitInterval);
			}

			// green and blue light intensity increase	
			for (var n = 0; n < 256; n++)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				g = (byte)n;
				led.SetPixelsRgb(r, g, b);
				led.Render();
				Thread.Sleep(waitInterval);

				b = (byte)n;
				led.SetPixelsRgb(r, g, b);
				led.Render();
				Thread.Sleep(waitInterval);
			}
		}
	}
}