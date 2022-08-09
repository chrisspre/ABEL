namespace abel;

public static class ExpressionEvaluator
{
    public static object Evaluate(this Expression expression)
    {
        return expression.Fold(new EvaluateFold());
    }

    private class EvaluateFold : Expression.IFold<object>
    {

        public object Integer(int value) => value;

        public object String(string value) => value;

        public object Binary(Operator @operator, object lhs, object rhs)
        {
            var fn = GetOperFunc(@operator);

            return fn(lhs, rhs);
        }

#pragma warning disable CS8524
        // disable CS8524 so that CS8509 can detect missing named enums
        private static Func<object, object, object> GetOperFunc(Operator op) => op switch
        {
            Operator.Mul => (a, b) => ((int)a) * ((int)b),
            Operator.Add => (a, b) => ((int)a) + ((int)b),
            Operator.Lt => (a, b) => ((int)a) < ((int)b),
            Operator.Lte => (a, b) => ((int)a) <= ((int)b),
        };

#pragma warning restore CS8524
    }
}