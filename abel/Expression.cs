namespace abel;


public enum Operator { Add, Mul }


public abstract record Expression
{
    protected Expression(ExpressionKind Kind) { }

    public enum ExpressionKind
    {
        Integer,
        String,
        Binary,
    }

    public abstract T Fold<T>(Func<int, T> NumberFn, Func<string, T> StringFn, Func<Operator, T, T, T> BinaryFn);

    public record Integer(int Value) : Expression(ExpressionKind.Integer)
    {
        public override T Fold<T>(Func<int, T> NumberFn, Func<string, T> StringFn, Func<Operator, T, T, T> BinaryFn)
        {
            return NumberFn(this.Value);
        }
    }

    public record String(string Value) : Expression(ExpressionKind.String)
    {
        public override T Fold<T>(Func<int, T> NumberFn, Func<string, T> StringFn, Func<Operator, T, T, T> BinaryFn)
        {
            return StringFn(this.Value);
        }
    }

    public record Binary(Operator Op, Expression Lhs, Expression Rhs) : Expression(ExpressionKind.Binary)
    {
        public override T Fold<T>(Func<int, T> NumberFn, Func<string, T> StringFn, Func<Operator, T, T, T> BinaryFn)
        {
            return BinaryFn(this.Op, this.Lhs.Fold(NumberFn, StringFn, BinaryFn), this.Rhs.Fold(NumberFn, StringFn, BinaryFn));
        }
    }

}