namespace abel;
using System.Text;

public abstract record ExpressionType
{
    protected ExpressionType(ExpressionTypeKind Kind) { this.Kind = Kind; }

    public enum ExpressionTypeKind
    {
        Boolean,
        Integer,
        String,
        Record,
    }

    public ExpressionTypeKind Kind { get; }

    public abstract T Fold<T>(IFold<T> fold);

    public interface IFold<T> { T Boolean(); T Integer(); T String(); T Record(IReadOnlyDictionary<string, T> fields); }


    public record Boolean() : ExpressionType(ExpressionTypeKind.Boolean)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Boolean();
        }
    }

    public record Integer() : ExpressionType(ExpressionTypeKind.Integer)
    {
        public override T Fold<T>(IFold<T> folder)
        {
            return folder.Integer();
        }
    }

    public record String() : ExpressionType(ExpressionTypeKind.String)
    {
        public override T Fold<T>(IFold<T> folder)
        {
            return folder.String();
        }
    }

    public record Record(IReadOnlyDictionary<string, ExpressionType> Fields) : ExpressionType(ExpressionTypeKind.Record)
    {
        public override T Fold<T>(IFold<T> folder)
        {
            return folder.Record(this.Fields.ToDictionary(p => p.Key, p => p.Value.Fold(folder)));
        }

        protected override bool PrintMembers(StringBuilder builder)
        {
            builder.AppendJoin(", ", from field in Fields select $"{field.Key}: {field.Value}");
            return Fields.Any() || base.PrintMembers(builder);
        }
    }
}