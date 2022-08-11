namespace abel;

public static class TypeExtensions
{
    public static string Display(this ExpressionType type)
    {
        return type.Fold(new DisplayFolder());
    }

    private class DisplayFolder : ExpressionType.IFold<string>
    {
        public string Boolean() => "Boolean";

        public string Integer() => "Integer";

        public string String() => "String";
        public string DateTime() => "DateTime";
        public string Period() => "Period";

        public string Record(IReadOnlyDictionary<string, string> fields)
        {
            // please note that the dictionary already contains the folded (i.e. displayed) field types
            // i.e. the dictionary value is a string, not a ExpressionType.
            return $"record {{ {string.Join(", ", from field in fields select $"{field.Key}: {field.Value}")} }}";
        }
    }
}
