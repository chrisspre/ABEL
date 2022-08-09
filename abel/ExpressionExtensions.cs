namespace abel;


public static class ExpressionExtensions
{
    public static string Display(this Expression expression)
    {
        (var p, var s) = expression.Fold(
            i => (9, i.ToString()),
            v => (9, v),
            Combine
        );
        return s;
    }


    private static (int, string) Combine(Operator oper, (int, string) lhs, (int, string) rhs)
    {
        var (operString, operPrio) = oper.SymAndPrio();
        string Parens((int, string) pair) => (pair.Item1 >= operPrio) ? pair.Item2 : $"({pair.Item2})";

        return (operPrio, $"{Parens(lhs)} {oper.Sym()} {Parens(rhs)}");
    }

    public static (string, int) SymAndPrio(this Operator op) => op switch
    {
        Operator.Add => ("+", 7),
        Operator.Mul => ("*", 8),
        _ => throw new ArgumentException("unnamed enum value")
    };

    public static string Sym(this Operator op) => op switch
    {
        Operator.Add => "+",
        Operator.Mul => "*",
        _ => throw new ArgumentException("unnamed enum value")
    };
}
