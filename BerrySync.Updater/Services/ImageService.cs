using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BerrySync.Updater.Services
{
    public class ImageService : IImageService
    {
        private ILogger<ImageService> _logger;

        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessCalendar()
        {
            var dir = $"{Constants.WorkDir}/crop";
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            
            Rectangle cornerOffset;
            Rectangle entry;
            Rectangle offset;

            using (var img = await Image.LoadAsync<L8>(Constants.CalendarFile))
            {
                img.Mutate(x =>
                {
                    x.Contrast(5);
                });
                cornerOffset = MathHelper.GetCornerOffset(img);
                entry = MathHelper.GetEntry(img, cornerOffset.Left, cornerOffset.Top);
                offset = MathHelper.GetOffset(img, entry.Left, entry.Top);
            }

            await CropCalendar(cornerOffset, entry, offset);
        }

        public async Task CropCalendar(Rectangle cornerOffset, Rectangle entry, Rectangle offset)
        {
            using (var img = await Image.LoadAsync<Rgb24>(Constants.CalendarFile))
            {
                var y = 0;
                var yCoord = MathHelper.CalculateY(img.Height, cornerOffset.Height, entry.Height, offset.Height, y);
                while (yCoord > 0)
                {
                    for (var x = 0; x < 7; x++)
                    {
                        using (var date = img.Clone())
                        {
                            date.Mutate(i =>
                            {
                                var xCoord = MathHelper.CalculateX(img.Width, cornerOffset.Width, entry.Width, offset.Width, x);
                                i.Crop(new Rectangle(xCoord, yCoord, entry.Width, entry.Height));
                            });

                            if (MathHelper.HasText(date))
                            {
                                using (var top = date.Clone())
                                {
                                    top.Mutate(i =>
                                    {
                                        i.Contrast(1.5f);
                                        i.Crop(new Rectangle(0, 0, top.Width, top.Height / 2));
                                    });

                                    await SaveDate(top, $"{7 * y + x}");
                                }

                                using (var bottom = date.Clone())
                                {
                                    bottom.Mutate(i =>
                                    {
                                        i.Contrast(1.5f);
                                        i.Crop(new Rectangle(0, bottom.Height / 2, bottom.Width, bottom.Height / 2));
                                    });

                                    await SaveDate(bottom, $"{x}b");
                                }
                            }
                        }
                    }

                    yCoord = MathHelper.CalculateY(img.Height, cornerOffset.Height, entry.Height, offset.Height, ++y);
                }
            }
        }

        private static async Task SaveDate(Image<Rgb24> image, string baseFileName)
        {
            if (MathHelper.HasText(image))
            {
                var lBound = MathHelper.FindLBound(image) - 5;

                using (var text = image.Clone())
                {
                    text.Mutate(i =>
                    {
                        i.Crop(new Rectangle(0, 0, lBound, text.Height));
                    });

                    await text.SaveAsJpegAsync($"{Constants.WorkDir}/crop/{baseFileName}-t.jpg");
                }

                using (var num = image.Clone())
                {
                    num.Mutate(i =>
                    {
                        i.Crop(new Rectangle(lBound, 0, num.Width - lBound, num.Height));
                    });

                    await num.SaveAsJpegAsync($"{Constants.WorkDir}/crop/{baseFileName}-n.jpg");
                }
            }
        }
    }
}
