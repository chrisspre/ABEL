using System.Text.RegularExpressions;

namespace abel.parsing;



public class Tokenizer
{
    private readonly Regex regex;

    private readonly Dictionary<string, TokenKind> TokenNameMap;

    private readonly (string Name, string Regex)[] Tokens;

    const RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture;


    public Tokenizer()
    {
        this.TokenNameMap = Enum.GetValues<TokenKind>().ToDictionary(k => Enum.GetName<TokenKind>(k)!, k => k);
        this.Tokens = new (string Name, string Regex)[] {
            ("Whitespace", "\\s+"),
            ("Ident", "[_a-zA-Z][_a-zA-Z0-9]*"),
            ("Integer", "[0-9]+"),
            ("PlusSign", "[+]"),
            ("Asterisk", "[*]"),
            ("LeftParenthesis", "[(]"),
            ("RightParenthesis", "[)]"),
            ("Unkown", ".+"),
        };

        var regex = "(" + string.Join("|", from token in Tokens select $"(?<{token.Name}>{token.Regex})") + ")";
        this.regex = new Regex(regex, Options);
    }

    public IEnumerable<Token> Tokenize(string input, bool ignoreWhitespace = true)
    {
        var pos = (1, 1);
        foreach (Match m in regex.Matches(input))
        {
            // only one top level alternative (group) can be a success
            var matchingGroup = m.Groups.Cast<Group>().Single(g => g.Success && g.Name != "0");
            if (!(ignoreWhitespace && matchingGroup.Name == "Whitespace"))
            {
                if (TokenNameMap.TryGetValue(matchingGroup.Name, out var kind))
                {
                    yield return new Token(kind, matchingGroup.Value, pos);
                }
                else
                {
                    throw new TokenizerException($"unkown matching group {matchingGroup.Name}");
                }
            }
            pos = UpdatedPosition(pos, m.Value);
        }
    }

    private (int Ln, int Col) UpdatedPosition((int Ln, int Col) pos, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '\r':
                    if (text.Length >= i + 1 && text[i + 1] == '\n')
                    {
                        i++;
                    }
                    pos = (pos.Ln + 1, 1);
                    break;
                case '\n':
                    pos = (pos.Ln + 1, 1);
                    break;
                default:
                    pos = (pos.Ln, pos.Col + 1);
                    break;
            }
        }
        return pos;
    }
}

