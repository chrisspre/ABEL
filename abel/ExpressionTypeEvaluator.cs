namespace abel;

using K = ExpressionType.ExpressionTypeKind;
using O = Operator;

public static class ExpressionTypeEvaluator
{
    public static ExpressionType ExpressionType(this Expression expression)
    {
        return expression.Fold(new TypeInferenceFold());
    }

    private class TypeInferenceFold : Expression.IFold<ExpressionType>
    {

        public ExpressionType Integer(int value) => new ExpressionType.Integer();

        public ExpressionType String(string value) => new ExpressionType.String();

        public ExpressionType Boolean(bool value) => new ExpressionType.Boolean();

        public ExpressionType Binary(Operator @operator, ExpressionType lhs, ExpressionType rhs)
        {

#pragma warning disable CS8524
            // disable CS8524 so that CS8509 can detect missing named enums
            return (lhs.Kind, @operator, rhs.Kind) switch
            {
                (K.Integer, O.Mul, K.Integer) => new ExpressionType.Integer(),

                (K.Integer, O.Add, K.Integer) => new ExpressionType.Integer(),

                (K.Integer, O.Lt, K.Integer) => new ExpressionType.Boolean(),
                (K.String, O.Lt, K.String) => new ExpressionType.Boolean(),

                (K.Integer, O.Lte, K.Integer) => new ExpressionType.Boolean(),
                (K.String, O.Lte, K.String) => new ExpressionType.Boolean(),

                _ => throw new NotSupportedException($"{lhs.Kind} {@operator} {rhs.Kind}")
                // TODO! better error handling. instead of throwing exception this should return an error value
            };

#pragma warning restore CS8524
        }
    }

}