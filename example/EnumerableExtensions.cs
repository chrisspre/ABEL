static class EnumerableExtensions
{
    public static string List<T>(this IEnumerable<T> items, Func<T, string> selector, string separator = ", ")
    {
        return string.Join(separator, items.Select(selector));
    }
}
