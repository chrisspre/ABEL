using System.Diagnostics;
using abel;
using abel.parsing;

// record User(string First, string Last, int Age);



class Program
{
    private static void Main(string[] args)
    {
        DemoTokenizer();

        // DemoExpressionTrees();
    }

    private static void DemoExpressionTrees()
    {
        var e = new Expression.Binary(Operator.Mul,
                    new Expression.Binary(Operator.Add,
                        new Expression.Integer(2),
                        new Expression.String("x")),
                    new Expression.String("y"));
        ShowExpression(e);

        var e2 = new Expression.Binary(Operator.Add, new Expression.Binary(Operator.Mul, new Expression.Integer(2), new Expression.Integer(3)), new Expression.Integer(4));
        ShowExpression(e2);


        var t = new ExpressionType.Record(new Dictionary<string, ExpressionType>
        {
            ["first"] = new ExpressionType.String(),
            ["age"] = new ExpressionType.Integer()
        });
        ShowExpressionType(t);
    }

    private static void DemoTokenizer()
    {
        var tokenizer = new Tokenizer(); // building the tokenizer actually takes substantial time (compiling a regex)
        var sw = Stopwatch.StartNew();
        var tokens = tokenizer.Tokenize(@"12 +
            (222 * 33)").ToList();
        sw.Stop();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        System.Console.WriteLine("time: {0}", sw.Elapsed);
    }

    private static void ShowExpression(Expression.Binary e)
    {
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
    }


    private static void ShowExpressionType(ExpressionType t)
    {
        Console.WriteLine("debug:   {0}", t);
        Console.WriteLine("display: {0}", t.Display());
        Console.WriteLine();
    }
}

