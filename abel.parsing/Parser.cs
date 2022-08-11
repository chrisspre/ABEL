using System.Diagnostics.CodeAnalysis;

namespace abel.parsing;

public class Parser
{
    // https://users.monash.edu/~lloyd/tildeProgLang/Grammar/Arith-Exp/
    //     <Exp> ::= <Exp> + <Term> 
    //             | <Exp> - <Term> 
    //             | <Term>
    // <Term>    ::= <Term> * <Factor> 
    //             | <Term> / <Factor> 
    //             | <Factor>
    // <Factor>  ::= x | y | ... 
    //             | ( <Exp> ) 
    //             | - <Factor>
    public bool TryParseExpression(ReadOnlySpan<Token> input, [MaybeNullWhen(false)] out abel.Expression value, [MaybeNullWhen(false)] out ReadOnlySpan<Token> remainder)
    {
        value = default;
        remainder = default;

        if (TryParseTerm(input, out var res, out var rem))
        {
            while (rem.Length > 0 && (rem[0].Kind == TokenKind.PlusSign || rem[0].Kind == TokenKind.MinusSign))
            {
                var op = rem[0].Kind == TokenKind.PlusSign ? Operator.Add : Operator.Sub;
                rem = rem.Slice(1);
                if (TryParseFactor(rem, out var factor, out rem))
                {

                    res = new Expression.Binary(op, res, factor);
                }
                else
                {
                    return false;
                }
            }

            value = res;
            remainder = rem;
            return true;
        }

        return false;
    }

    public bool TryParseTerm(ReadOnlySpan<Token> input, [MaybeNullWhen(false)] out abel.Expression value, [MaybeNullWhen(false)] out ReadOnlySpan<Token> remainder)
    {
        value = default;
        remainder = default;

        if (TryParseFactor(input, out var res, out var rem))
        {
            while (rem.Length > 0 && rem[0].Kind == TokenKind.Asterisk) // TODO, or division
            {
                rem = rem.Slice(1);
                if (TryParseTerm(rem, out var term, out rem))
                {
                    res = new Expression.Binary(Operator.Mul, res, term);
                }
                else
                {
                    return false;
                }
            }

            value = res;
            remainder = rem;
            return true;
        }

        return false;
    }

    public bool TryParseFactor(ReadOnlySpan<Token> input, [MaybeNullWhen(false)] out abel.Expression value, [MaybeNullWhen(false)] out ReadOnlySpan<Token> remainder)
    {
        var head = input[0];
        if (head.Kind == TokenKind.True)
        {
            value = new Expression.Boolean(true);
            remainder = input.Slice(1);
            return true;
        }
        else if (head.Kind == TokenKind.False)
        {
            value = new Expression.Boolean(false);
            remainder = input.Slice(1);
            return true;
        }
        else if (head.Kind == TokenKind.Integer)
        {
            value = new Expression.Integer(Int32.Parse(head.Value)); // TODO check overflow
            remainder = input.Slice(1);
            return true;
        }

        value = default;
        remainder = default;
        return false;
    }

    public static bool TryParse(string input, [MaybeNullWhen(false)] out Expression expr)
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize(input).ToArray();
        var parser = new Parser();

        return parser.TryParseExpression(tokens, out expr, out var rem);
    }
}

