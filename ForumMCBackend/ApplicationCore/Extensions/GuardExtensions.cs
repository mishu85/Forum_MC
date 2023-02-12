using ApplicationCore.Entities;
using ApplicationCore.Exeptions;

namespace Ardalis.GuardClauses;

public static class MessageGuards
{
    public static void NoRepliesToReplies(this IGuardClause guardClause, Message inReplyTo)
    {
        if (inReplyTo.InReplyTo?.InReplyToId != null)
            throw new NoRepliesToRepliesException();
    }
}
