namespace abel;
using K = ExpressionType.ExpressionTypeKind;

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
        public object DateTime(Expression.DateTime dateTime) => dateTime.Value;
        public object Period(Expression.Period period) => period.Value;

        public object Binary(Expression.Binary binary, object lhs, object rhs)
        {
            var lType = GetTypeCode(lhs);
            var rType = GetTypeCode(rhs);
            var fn = GetOperFunc(binary.Op, lType, rType);

            return fn(lhs, rhs);
        }

#pragma warning disable CS8524
        // disable CS8524 so that CS8509 can detect missing named enums
        private static Func<object, object, object> GetOperFunc(Operator op, ExpressionType.ExpressionTypeKind lhs, ExpressionType.ExpressionTypeKind rhs)
        {
            return (lhs, op, rhs) switch
            {
                (K.Integer, Operator.Mul, K.Integer) => (a, b) => ((int)a) * ((int)b),
                (K.Integer, Operator.Add, K.Integer) => (a, b) => ((int)a) + ((int)b),
                (K.Integer, Operator.Sub, K.Integer) => (a, b) => ((int)a) - ((int)b),
                (K.Integer, Operator.Lt, K.Integer) => (a, b) => ((int)a) < ((int)b),
                (K.Integer, Operator.Lte, K.Integer) => (a, b) => ((int)a) <= ((int)b),

                (K.DateTime, Operator.Add, K.Period) => (a, b) => ((NodaTime.LocalDateTime)a) + ((NodaTime.Period)b),
                _ => throw new NotImplementedException($"nod operation for  {lhs} {op} {rhs}") // TODO: better error handling
            };
        }

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



    // enum TypeCode { Boolean, Integer, String, DateTime, Period, Record }

    private static ExpressionType.ExpressionTypeKind GetTypeCode(object lhs)
    {
        return Type.GetTypeCode(lhs.GetType()) switch
        {
            System.TypeCode.Boolean => ExpressionType.ExpressionTypeKind.Boolean,
            System.TypeCode.String => ExpressionType.ExpressionTypeKind.String,
            System.TypeCode.Int32 => ExpressionType.ExpressionTypeKind.Integer,
            _ => lhs switch
            {
                NodaTime.LocalDateTime => ExpressionType.ExpressionTypeKind.DateTime,
                NodaTime.Period => ExpressionType.ExpressionTypeKind.Period,
                _ => ((ExpressionType.ExpressionTypeKind)int.MaxValue),
            }
        };
    }
}