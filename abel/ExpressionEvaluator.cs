namespace abel;

public static class ExpressionEvaluator
{
    public static object Evaluate(this Expression expression, IReadOnlyDictionary<string, object> root)
    {
        return expression.Fold(new EvaluateFold(root));
    }

    private class EvaluateFold : Expression.IFold<object>
    {
        private IReadOnlyDictionary<string, object> root;

        public EvaluateFold(IReadOnlyDictionary<string, object> root)
        {
            this.root = root;
        }

        public object Integer(Expression.Integer integer) => integer.Value;

        public object String(Expression.String @string) => @string.Value;

        public object Boolean(Expression.Boolean boolean) => boolean.Value;

        public object Binary(Expression.Binary binary, object lhs, object rhs)
        {
            var fn = GetOperFunc(binary.Op);

            return fn(lhs, rhs);
        }

#pragma warning disable CS8524
        // disable CS8524 so that CS8509 can detect missing named enums
        private static Func<object, object, object> GetOperFunc(Operator op) => op switch
        {
            Operator.Mul => (a, b) => ((int)a) * ((int)b),
            Operator.Add => (a, b) => ((int)a) + ((int)b),
            Operator.Sub => (a, b) => ((int)a) - ((int)b),
            Operator.Lt => (a, b) => ((int)a) < ((int)b),
            Operator.Lte => (a, b) => ((int)a) <= ((int)b),
        };

        public object Self(Expression.Self self)
        {
            return root;
        }
#pragma warning restore CS8524

        public object MemberGet(Expression.MemberGet get, object obj)
        {
            if (obj is IReadOnlyDictionary<string, object> dict)
            {
                if (dict.TryGetValue(get.Member, out var val))
                {
                    return val;
                }
            }
            throw new InvalidCastException($"object {obj} doesn't have a property named {get.Member}"); // TODO: custom exception
        }
    }
}