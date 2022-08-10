using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using abel;
using abel.parsing;

record User(string First, string Last, int Age);

class Program
{
    private static void Main(string[] args)
    {
        // DemoTokenizer();

        DemoExpressionTrees();

        DemoEvaluation();
    }


    private static void DemoExpressionTrees()
    {
        var inputType = ExpressionType.Record.Empty;
        var inputData = new Dictionary<string, object>();

        ShowExpressionType(inputType);

        var e = new Expression.Binary(Operator.Mul,
                    new Expression.Binary(Operator.Add,
                        new Expression.Integer(2),
                        new Expression.String("x")),
                    new Expression.String("y"));
        ShowExpression(e, inputType, inputData);

        var e2 = new Expression.Binary(Operator.Add, new Expression.Binary(Operator.Mul, new Expression.Integer(2), new Expression.Integer(3)), new Expression.Integer(4));
        ShowExpression(e2, inputType, inputData);
    }

    private static void DemoEvaluation()
    {
        var inputType = new ExpressionType.Record
        {
            ["First"] = new ExpressionType.String(),
            ["Age"] = new ExpressionType.Integer()
        };

        var inputData = new User("Fred", "Flintstone", 23).ToRecord(inputType);

        var e = new Expression.Binary(
                Operator.Lte,
                new Expression.Integer(18),
                new Expression.MemberGet(new Expression.Self(), "Age")
        );

        ShowExpression(e, inputType, inputData);
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

    private static void ShowExpression(Expression.Binary e, ExpressionType.Record inputType, IReadOnlyDictionary<string, object> input)
    {
        Console.WriteLine("##########################");

        Console.WriteLine("debug:    {0}", e);
        Console.WriteLine("display:  {0}", e.Display());
        if (e.TryGetExpressionType(inputType, out var type, out var error))
        {
            Console.WriteLine("type:     {0}", type);

            Console.WriteLine("eval:     {0}", e.Evaluate(input));
        }
        else
        {
            Console.WriteLine($"type:     can't infer type: {error}");
            // don't attempt evaluation when the types are wrong.
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
