namespace abel;


public static class ExpressionExtensions
{
    public static string Display(this Expression expression)
    {
        return expression.Fold(new DisplayFold()).Item2;
    }

    private class DisplayFold : Expression.IFold<(int, string)>
    {
        public (int, string) Integer(int value) => (9, value.ToString());

        public (int, string) String(string value) => (9, value);

        public (int, string) Binary(Operator @operator, (int, string) lhs, (int, string) rhs)
        {
            var (operSym, operPrio) = @operator.SymAndPrio();
            string Parens((int, string) pair) => (pair.Item1 >= operPrio) ? pair.Item2 : $"({pair.Item2})";

            return (operPrio, $"{Parens(lhs)} {operSym} {Parens(rhs)}");
        }
    }


#pragma warning disable CS8524
    // CS8524 warns on unnamed enum values, which requires to add a default (_ => ...) case
    // which then in turn hides if a named enum value is not implemented. 
    // Disabling it lights up CS8509 which warns on missing named enums. 
    private static (string, int) SymAndPrio(this Operator op) => op switch
    {
        Operator.Mul => ("*", 8),
        Operator.Add => ("+", 7),
        Operator.Lt => ("<", 4),
        Operator.Lte => ("<=", 4),
    };
#pragma warning restore format

}
