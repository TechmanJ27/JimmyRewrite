using NetCord;

namespace JimmyRewrite;

public static class ToStringUtil
{
    public static string AttachmentsToString(IReadOnlyList<Attachment> attachments)
    {
        return string.Join(" ", attachments.Select(x => x.Url));
    }
}