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

        // DemoRecordEvaluation();
    }


    private static void DemoExpressionTrees()
    {
        var inputType = ExpressionType.Record.Empty;
        var inputData = new Dictionary<string, object>();

        var e1 = new Expression.Binary(Operator.Add, new Expression.Binary(Operator.Mul, new Expression.Integer(2), new Expression.Integer(3)), new Expression.Integer(4));
        ShowExpression(e1, inputType, inputData);

        var e2 = new Expression.Binary(Operator.Mul,
            new Expression.Binary(Operator.Add,
                new Expression.Integer(2),
                new Expression.String("x")),
            new Expression.String("y"));
        ShowExpression(e2, inputType, inputData);
    }

    private static void DemoRecordEvaluation()
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
        Console.WriteLine("## expression ");
        Console.WriteLine("debug:        {0}", e);
        Console.WriteLine("display:      {0}", e.Display());
        if (inputType != ExpressionType.Record.Empty)
        {
            Console.WriteLine("input:        {0}", input.List(p => $"{p.Key}: {p.Value}"));
            Console.WriteLine("input type:   {0}", inputType.Display());
        }


        if (e.TryGetExpressionType(inputType, out var type, out var error))
        {
            Console.WriteLine("expr type:    {0}", type);
            var sw = Stopwatch.StartNew();
            var result = e.Evaluate(input);
            sw.Stop();
            Console.WriteLine("evaluates to: {0}  (in {1}", result, sw.Elapsed);
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
        Console.WriteLine("##########################");
        Console.WriteLine("## expression type");
        Console.WriteLine("debug:   {0}", t);
        Console.WriteLine("display: {0}", t.Display());
        Console.WriteLine();
    }
}
