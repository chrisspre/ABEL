// See https://aka.ms/new-console-template for more information
namespace abel;

public static class TypeExtensions
{
    public static string Display(this ExpressionType type)
    {
        return type.Fold(
            () => "Boolean",
            () => "Integer",
            () => "String",
            f => $"record {{ {string.Join(", ", from nt in f select $"{nt.Key}: {nt.Value}")} }}"
        );
    }
}
