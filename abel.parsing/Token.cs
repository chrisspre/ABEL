using System.Text;

namespace abel.parsing;


public sealed record Token(string Kind, string Value, (int, int) Position)
{
    private bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Kind = {0}", Kind);
        builder.Append(", ");
        builder.AppendFormat("Value = \"{0}\"", Value);
        builder.Append(", ");
        builder.AppendFormat("Position = {0}", Position);
        return true;
    }
}
