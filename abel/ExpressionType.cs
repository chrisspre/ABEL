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
        DateTime,
        Period,
        Record,
    }

    public ExpressionTypeKind Kind { get; }

    public abstract T Fold<T>(IFold<T> fold);

    public interface IFold<T> { T Boolean(); T Integer(); T String(); T DateTime(); T Period(); T Record(IReadOnlyDictionary<string, T> fields); }

    public record Boolean() : ExpressionType(ExpressionTypeKind.Boolean)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Boolean();
        }
    }

    public record Integer() : ExpressionType(ExpressionTypeKind.Integer)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Integer();
        }
    }

    public record String() : ExpressionType(ExpressionTypeKind.String)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.String();
        }
    }

    public record DateTime() : ExpressionType(ExpressionTypeKind.DateTime)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.DateTime();
        }
    }

    public record Period() : ExpressionType(ExpressionTypeKind.Period)
    {
        public override T Fold<T>(IFold<T> fold)
        {
            return fold.Period();
        }
    }

    public record Record() : ExpressionType(ExpressionTypeKind.Record)
    {
        private readonly Dictionary<string, ExpressionType> fields = new Dictionary<string, ExpressionType>();

        public static Record Empty = new Record();

        public IReadOnlyDictionary<string, ExpressionType> Fields => fields;
        public Record(IReadOnlyDictionary<string, ExpressionType> fields) : this()
        {
            this.fields = fields.ToDictionary(p => p.Key, p => p.Value);
        }

        public ExpressionType this[string name]
        {
            get => Fields[name];
            init => fields[name] = value;
        }

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