using Discord.Interactions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Images;

public abstract class Gift : InteractionModuleBase<SocketInteractionContext>
{
    public async static Task GiftCmd(SocketInteractionContext context, string imageUrl)
    {
        await context.Interaction.DeferAsync();

        var (errorMsg, bitmap) = await Image.GetImageFromUrl(imageUrl);
        if (bitmap == null)
        {
            await context.Interaction.FollowupAsync(errorMsg, ephemeral: true);
            return;
        }

        bitmap = Image.ResizeBitmap(bitmap, 400);
        var kalousek = Image.LoadBitmapFromProject(Path.Combine("Img", "ImgCmd", "kalousek.png"));

        var combinedBitmaps = Image.CombineBitmaps(kalousek, bitmap, (170, 270));

        await context.Interaction.FollowupWithFileAsync(combinedBitmaps.ToMemoryStream(),
            "kalousek.png",
            "**Kalousek would like to give you a gift!**");
    }
}
