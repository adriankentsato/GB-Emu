using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyEmu
{
    [StructLayout(LayoutKind.Explicit)]
    struct Pixel
    {
        [FieldOffset(3)] public byte r;
        [FieldOffset(2)] public byte g;
        [FieldOffset(1)] public byte b;
        [FieldOffset(0)] public byte a;
        [FieldOffset(0)] public UInt32 rgba;
    }

    internal class Graphics
    {
        public const int MAX_HEIGHT = 160;
        public const int MAX_WIDTH = 144;

        private Pixel[][] pixels;

        public Graphics()
        {
            pixels = new Pixel[Graphics.MAX_WIDTH][];

            for (int i = 0; i < Graphics.MAX_WIDTH; i++)
            {
                pixels[i] = new Pixel[Graphics.MAX_HEIGHT];
            }

            pixels[50][50].rgba = 0xFF0000FF;
        }

        public void Draw(System.Drawing.Graphics g)
        {
            int scaleX = (int)Math.Floor(g.ClipBounds.Width / Graphics.MAX_WIDTH);
            int scaleY = (int)Math.Floor(g.ClipBounds.Height / Graphics.MAX_HEIGHT);

            SolidBrush b = new SolidBrush(Color.Black);

            g.Clear(Color.White);

            for (int x = 0; x < Graphics.MAX_WIDTH; x++)
            {
                for (int y = 0; y < Graphics.MAX_HEIGHT; y++)
                {
                    b.Color = Color.FromArgb(pixels[x][y].a, pixels[x][y].r, pixels[x][y].g, pixels[x][y].b);
                    g.FillRectangle(b, x * scaleX, y * scaleY, scaleX, scaleY);
                }
            }

        }
    }
}
