namespace abel;


public enum Operator { Add, Sub, Mul, Lt, Lte }


public abstract record Expression
{
    protected Expression(ExpressionKind Kind) { }

    public enum ExpressionKind
    {
        Integer,
        String,
        Boolean,
        DateTime,
        Period,

        Binary,

        Self,
        MemberGet,
    }

    // public abstract T Fold<T>(IFold<T> fold);
    public interface IFold<T>
    {
        T Boolean(Boolean boolean);
        T Integer(Integer value);
        T String(String value);
        T DateTime(DateTime value);
        T Period(Period value);

        T Binary(Binary binary, T lhs, T rhs);

        T Self(Self self);
        T MemberGet(MemberGet get, T obj);
    }

    public abstract T Fold<T>(IFold<T> fold);

    public record Integer(int Value) : Expression(ExpressionKind.Integer)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Integer(this);
        }
    }

    public record Boolean(bool Value) : Expression(ExpressionKind.Boolean)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Boolean(this);
        }
    }

    public record String(string Value) : Expression(ExpressionKind.String)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.String(this);
        }
    }
    public record DateTime(NodaTime.LocalDateTime Value) : Expression(ExpressionKind.String)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.DateTime(this);
        }
    }

    public record Period(NodaTime.Period Value) : Expression(ExpressionKind.String)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Period(this);
        }
    }

    public record Binary(Operator Op, Expression Lhs, Expression Rhs) : Expression(ExpressionKind.Binary)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Binary(this, this.Lhs.Fold(fold), this.Rhs.Fold(fold));
        }
    }

    public record Self() : Expression(ExpressionKind.Self)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Self(this);
        }
    }

    public record MemberGet(Expression Obj, string Member) : Expression(ExpressionKind.MemberGet)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.MemberGet(this, this.Obj.Fold(fold));
        }
    }
}