using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BerrySync.Updater
{
    public class MathHelper
    {
        public static Rectangle GetCornerOffset(Image<L8> img)
        {
            var x = img.Width * 97 / 100;
            var y = img.Height - 1;

            while (!IsDark(img[x, y]))
            {
                y--;
            }

            while (x + 1 < img.Width && IsDark(img[x + 1, y]))
            {
                x++;
            }
            return new Rectangle(x, y, img.Width - x - 1, img.Height - y - 1);
        }

        public static Rectangle GetEntry(Image<L8> img, int startX, int startY)
        {
            var x = startX;
            var y = startY;

            while (IsDark(img[x, y - 1])
                || IsDark(img[x - 1, y - 1]))
            {
                y--;
            }

            while (IsDark(img[x - 1, y])
                || IsDark(img[x - 1, y + 1]))
            {
                x--;
            }

            return new Rectangle(x, y, startX - x + 1, startY - y + 1);
        }

        public static Rectangle GetOffset(Image<L8> img, int startX, int startY)
        {
            var x = startX;
            var y = startY;

            while (!IsDark(img[x, --y])) ;
            while (!IsDark(img[--x, y])) ;

            return new Rectangle(x, y, startX - x - 1, startY - y - 1);
        }

        public static bool IsDark(L8 p)
        {
            return p.PackedValue <= 128;
        }

        public static int CalculateX(int image, int corner, int entry, int offset, int x)
        {
            return Math.Max(image - corner - (x + 1) * entry - x * offset, 0);
        }

        public static int CalculateY(int image, int corner, int entry, int offset, int y)
        {
            return Math.Max(image - corner - (y + 1) * entry - y * offset, 0);
        }

        public static bool HasText(Image<Rgb24> img)
        {
            for (int x = 0; x < img.Width / 2; x++)
            {
                for (int y = 0; y < img.Height / 2; y++)
                {
                    var pxl = img[x, y];
                    if ((pxl.G == 0 || pxl.R / pxl.G >= 2)
                        || (pxl.B == 0 || pxl.R / pxl.B >= 2))
                        return true;
                }
            }
            return false;
        }
    }
}
