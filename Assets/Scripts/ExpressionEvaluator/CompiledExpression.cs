#nullable enable

namespace ExpressionEvaluator
{
    public class CompiledExpression
    {
        private readonly Expr _root;
        public string ExpressionString { get; private set; }

        internal CompiledExpression(Expr root, string expressionString)
        {
            _root = root;
            ExpressionString = expressionString;
        }

        public object? Evaluate(IAttributeContext ctx)
        {
            return _root.Eval(ctx);
        }
    }
}
