namespace abel;

public static class ExpressionExtenstions
{
    public static string Display(this Expression expression)
    {
        return expression.Fold(new DisplayFold()).Item2;
    }

    private class DisplayFold : Expression.IFold<(int, string)>
    {
        public (int, string) Binary(Expression.Binary binary, (int, string) lhs, (int, string) rhs)
        {
            var (operSym, operPrio) = binary.Op.SymAndPrio();
            string Parens((int, string) pair) => (pair.Item1 >= operPrio) ? pair.Item2 : $"({pair.Item2})";

            return (operPrio, $"{Parens(lhs)} {operSym} {Parens(rhs)}");
        }

        public (int, string) Boolean(Expression.Boolean boolean) => (9, boolean.Value.ToString());

        public (int, string) Integer(Expression.Integer integer) => (9, integer.Value.ToString());


        public (int, string) String(Expression.String @string) => (9, $"\"{@string.Value.ToString()}\"");

        public (int, string) MemberGet(Expression.MemberGet get, (int, string) obj) => (9, obj.Item2 + "." + get.Member);

        public (int, string) Self(Expression.Self self) => (9, "self");
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
#pragma warning restore CS8524

}