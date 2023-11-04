using Discord;

namespace Zaoshi.Exceptions;

/// <inheritdoc />
/// <summary>
///     Exception with ephemeral message sent to user
/// </summary>
public class UserException : Exception
{
    /// <inheritdoc />
    /// <summary>
    ///     Throws an exception stopping the interaction and giving info to the user in form of an ephemeral message
    /// </summary>
    /// <param name="context"></param>
    /// <param name="msg"></param>
    public UserException(IInteractionContext context, string msg) : base(msg)
    {
        context.Interaction.RespondAsync(msg);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Suppresses writing the exception to the output console
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Empty;
}
