namespace abel;


public enum Operator { Add, Mul, Lt, Lte }


public abstract record Expression
{
    protected Expression(ExpressionKind Kind) { }

    public enum ExpressionKind
    {
        Integer,
        String,
        Binary,
        Boolean
    }

    // public abstract T Fold<T>(IFold<T> fold);
    public interface IFold<T>
    {
        T Integer(int value);
        T String(string value);
        T Boolean(bool value);
        T Binary(Operator @operator, T lhs, T rhs);
    }

    public abstract T Fold<T>(IFold<T> fold);

    public record Integer(int Value) : Expression(ExpressionKind.Integer)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Integer(this.Value);
        }
    }

    public record Boolean(bool Value) : Expression(ExpressionKind.Boolean)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Boolean(this.Value);
        }
    }

    public record String(string Value) : Expression(ExpressionKind.String)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.String(this.Value);
        }
    }

    public record Binary(Operator Op, Expression Lhs, Expression Rhs) : Expression(ExpressionKind.Binary)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Binary(this.Op, this.Lhs.Fold(fold), this.Rhs.Fold(fold));
        }
    }
}