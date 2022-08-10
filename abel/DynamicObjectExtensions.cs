namespace abel;

public static class DynamicObjectExtensions
{
    public static IReadOnlyDictionary<string, object> ToRecord<T>(this T obj)
    {
        var dictionary = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj)!, StringComparer.InvariantCultureIgnoreCase);
        return dictionary;
    }

    public static IReadOnlyDictionary<string, object> ToRecord<T>(this T obj, ExpressionType.Record recordType)
    {
        var dictionary = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj)!, StringComparer.InvariantCultureIgnoreCase);
        foreach (var prop in recordType.Fields)
        {
            if (!dictionary.ContainsKey(prop.Key))
            {
                throw new ArgumentOutOfRangeException($"object is missing property {prop.Key}, {prop.Value}");
            }
        }
        return dictionary;
    }
}

