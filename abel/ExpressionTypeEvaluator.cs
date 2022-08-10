namespace abel;

using System.Diagnostics.CodeAnalysis;
using K = ExpressionType.ExpressionTypeKind;
using O = Operator;

public static class ExpressionTypeEvaluator
{
    public static bool TryGetExpressionType(this Expression expression, ExpressionType.Record root, [MaybeNullWhen(false)] out ExpressionType type, [MaybeNullWhen(true)] out string error)
    {
        try { type = expression.Fold(new TypeInferenceFold(root)); error = null; return true; }
        catch (TypeInferenceException ex) { type = default; error = ex.Message; return false; }
    }

    [Obsolete("use TryGetExpressionType")]
    public static ExpressionType ExpressionType(this Expression expression, ExpressionType.Record root)
    {
        return expression.Fold(new TypeInferenceFold(root));
    }

    private class TypeInferenceFold : Expression.IFold<ExpressionType>
    {
        private ExpressionType.Record selfType;

        public TypeInferenceFold(ExpressionType.Record selfType)
        {
            this.selfType = selfType;
        }

        public ExpressionType Integer(Expression.Integer integer) => new ExpressionType.Integer();

        public ExpressionType String(Expression.String @string) => new ExpressionType.String();

        public ExpressionType Boolean(Expression.Boolean boolean) => new ExpressionType.Boolean();

        public ExpressionType Binary(Expression.Binary binary, ExpressionType lhs, ExpressionType rhs)
        {
            return (lhs.Kind, binary.Op, rhs.Kind) switch
            {
                (K.Integer, O.Mul, K.Integer) => new ExpressionType.Integer(),

                (K.Integer, O.Add, K.Integer) => new ExpressionType.Integer(),

                (K.Integer, O.Lt, K.Integer) => new ExpressionType.Boolean(),
                (K.String, O.Lt, K.String) => new ExpressionType.Boolean(),

                (K.Integer, O.Lte, K.Integer) => new ExpressionType.Boolean(),
                (K.String, O.Lte, K.String) => new ExpressionType.Boolean(),

                _ => throw new TypeInferenceException($"operator {binary.Op} can't be used with operands of type {lhs.Kind}  and {rhs.Kind} in {binary.Display()}", binary)
            };
        }

        public ExpressionType Self(Expression.Self self)
        {
            return selfType;
        }

        public ExpressionType MemberGet(Expression.MemberGet get, ExpressionType objType)
        {
            if (objType is ExpressionType.Record record)
            {
                if (record.Fields.TryGetValue(get.Member, out var memberType))
                {
                    return memberType;
                }
                else
                {
                    throw new TypeInferenceException($"object of type {record} doesn't have a property named {get.Member}", get);
                }
            }
            else
            {
                throw new TypeInferenceException($"object of type {objType} doesn't have properties", get);
            }
        }
    }
}

public class TypeInferenceException : Exception
{
    public TypeInferenceException(string message, Expression e) : base(message)
    {
        Expression = e;
    }

    public Expression Expression { get; }
}