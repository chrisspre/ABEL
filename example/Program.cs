using abel;

internal class Program
{
    private static void Main(string[] args)
    {

        var e = new Expression.Binary(Operator.Mul, new Expression.Binary(Operator.Add, new Expression.Integer(2), new Expression.String("x")), new Expression.String("y"));

        Console.WriteLine(e);
        Console.WriteLine(e.Display());
        Console.WriteLine();

        e = new Expression.Binary(Operator.Add, new Expression.Binary(Operator.Mul, new Expression.Integer(2), new Expression.String("x")), new Expression.String("y"));

        Console.WriteLine(e);
        Console.WriteLine(e.Display());
        Console.WriteLine();


        ExpressionType t = new ExpressionType.Record(new Dictionary<string, ExpressionType>
        {
            ["first"] = new ExpressionType.String(),
            ["age"] = new ExpressionType.Integer()
        });

        Console.WriteLine(t);
        Console.WriteLine(t.Display());
        Console.WriteLine();
    }
}

