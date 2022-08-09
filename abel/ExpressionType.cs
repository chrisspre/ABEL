namespace abel;
using System.Text;

public abstract record ExpressionType
{
    protected ExpressionType(ExpressionTypeKind Kind) { }

    public enum ExpressionTypeKind
    {
        Boolean,
        Integer,
        String,
        Record,
    }

    public abstract T Fold<T>(Func<T> BooleanFn, Func<T> IntegerFn, Func<T> StringFn, Func<IReadOnlyDictionary<string, T>, T> RecordFn);

    public record Boolean() : ExpressionType(ExpressionTypeKind.Boolean)
    {
        public override T Fold<T>(Func<T> BooleanFn, Func<T> IntegerFn, Func<T> StringFn, Func<IReadOnlyDictionary<string, T>, T> RecordFn)
        {
            return BooleanFn();
        }
    }

    public record Integer() : ExpressionType(ExpressionTypeKind.Integer)
    {
        public override T Fold<T>(Func<T> BooleanFn, Func<T> IntegerFn, Func<T> StringFn, Func<IReadOnlyDictionary<string, T>, T> RecordFn)
        {
            return IntegerFn();
        }
    }

    public record String() : ExpressionType(ExpressionTypeKind.String)
    {
        public override T Fold<T>(Func<T> BooleanFn, Func<T> IntegerFn, Func<T> StringFn, Func<IReadOnlyDictionary<string, T>, T> RecordFn)
        {
            return StringFn();
        }
    }

    public record Record(IReadOnlyDictionary<string, ExpressionType> Fields) : ExpressionType(ExpressionTypeKind.Record)
    {
        public override T Fold<T>(Func<T> BooleanFn, Func<T> IntegerFn, Func<T> StringFn, Func<IReadOnlyDictionary<string, T>, T> RecordFn)
        {
            return RecordFn(this.Fields.ToDictionary(p => p.Key, p => p.Value.Fold(BooleanFn, IntegerFn, StringFn, RecordFn)));
        }

        protected override bool PrintMembers(StringBuilder builder)
        {
            builder.AppendJoin(", ", from field in Fields select $"{field.Key}: {field.Value}");
            return Fields.Any() || base.PrintMembers(builder);
        }
    }
}