using abel;

// record User(string First, string Last, int Age);



class Program
{
    private static void Main(string[] args)
    {

        var e = new Expression.Binary(Operator.Mul,
            new Expression.Binary(Operator.Add,
                new Expression.Integer(2),
                new Expression.String("x")),
            new Expression.String("y"));

        Console.WriteLine("debug:    {0}", e);
        Console.WriteLine("display:  {0}", e.Display());
        if (e.TryGetExpressionType(out var type, out var error))
        {
            Console.WriteLine("type:     {0}", type);
        }
        else
        {
            Console.WriteLine($"can't infer type: {error}");
        }
        Console.WriteLine();

        e = new Expression.Binary(Operator.Add, new Expression.Binary(Operator.Mul, new Expression.Integer(2), new Expression.Integer(3)), new Expression.Integer(4));

        Console.WriteLine("debug:    {0}", e);
        Console.WriteLine("display:  {0}", e.Display());
        Console.WriteLine("type:     {0}", e.ExpressionType());
        Console.WriteLine("evaluate: {0}", e.Evaluate());
        Console.WriteLine();


        ExpressionType t = new ExpressionType.Record(new Dictionary<string, ExpressionType>
        {
            ["first"] = new ExpressionType.String(),
            ["age"] = new ExpressionType.Integer()
        });

        Console.WriteLine("debug:   {0}", t);
        Console.WriteLine("display: {0}", t.Display());
        Console.WriteLine();
    }
}

