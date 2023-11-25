using Discord.Interactions;
using SkiaSharp;
using System.Reflection;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Images;

[Group("image", "All the commands working with images")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Image : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("gift", "Kalousek wants to give you something")]
    public async Task Gift(string imageUrl) => await Images.Gift.GiftCmd(Context, imageUrl);

    public async static Task<(string?, SKBitmap?)> GetImageFromUrl(string imageUrl)
    {
        if (Regex.Match(imageUrl, @"\.(png|jpg|jpeg|webp|gif)(\?.*)?$").Groups.Count <= 1)
        {
            return ("Only image links are supported, please try another one", null);
        }

        byte[] bytes;
        using (var httpClient = new HttpClient())
        {
            try
            {
                bytes = await httpClient.GetByteArrayAsync(imageUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return ("Invalid url", null);
            }
        }

        try
        {
            using var memoryStream = new MemoryStream(bytes);
            var skBitmap = SKBitmap.Decode(memoryStream);

            return (null, skBitmap);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ("Can't create image from the url, please try different one", null);
        }
    }

    public static SKBitmap ResizeBitmap(SKBitmap bitmap, int maxSize)
    {
        var originalSize = new SKSize(bitmap.Width, bitmap.Height);
        var resultSizePercent = Math.Min(maxSize / originalSize.Width, maxSize / originalSize.Height);
        var resultSize = new SKSizeI((int)Math.Floor(originalSize.Width * resultSizePercent), (int)Math.Floor(originalSize.Height * resultSizePercent));

        return bitmap.Resize(resultSize, SKFilterQuality.High);
    }

    public static SKBitmap CombineBitmaps(SKBitmap background, SKBitmap bitmap, (int, int) imgPosition)
    {
        using var canvas = new SKCanvas(background);
        canvas.DrawBitmap(bitmap, imgPosition.Item1, imgPosition.Item2);
        canvas.Save();

        return background;
    }

    public static SKBitmap LoadBitmapFromProject(string relativePath)
    {
        var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, relativePath);
        return SKBitmap.Decode(fullPath);
    }
}

public static class SKBitmapExtensions
{
    public static MemoryStream ToMemoryStream(this SKBitmap bitmap)
    {
        var ms = new MemoryStream();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(ms);

        return ms;
    }
}
