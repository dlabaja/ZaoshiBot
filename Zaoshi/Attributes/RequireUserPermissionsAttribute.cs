using Discord;
using Discord.Interactions;

namespace Zaoshi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireUserPermissionsAttribute : PreconditionAttribute
{
    public RequireUserPermissionsAttribute(GuildPermission[] guildPermissions)
    {
        GuildPermissions = guildPermissions;
    }

    private IEnumerable<GuildPermission> GuildPermissions { get; }

    public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        try
        {
            var user = context.User as IGuildUser ?? throw new InvalidOperationException("Command must be used in a guild channel");
            if (!GuildPermissions.Intersect(user.GuildPermissions.ToList()).Any()) throw new Exception("Missing Permissions");
            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
        catch (Exception e)
        {
            await context.Interaction.RespondAsync(e.Message, ephemeral: true);
            return await Task.FromResult(PreconditionResult.FromError(e.Message));
        }
    }
}
