using System.Text;

namespace abel.parsing;



public enum TokenKind
{
    Unkown,
    Whitespace,
    Ident,
    Integer,

    // names follow unicode naming for single character tokens
    PlusSign, // https://unicodeplus.com/U+002B
    Asterisk, // https://unicodeplus.com/U+002A
    LeftParenthesis, // https://unicodeplus.com/U+0028
    RightParenthesis, // https://unicodeplus.com/U+0029
}

public sealed record Token(TokenKind Kind, string Value, (int, int) Position)
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
