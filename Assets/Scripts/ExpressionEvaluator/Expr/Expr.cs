#nullable enable

namespace ExpressionEvaluator
{
    abstract class Expr
    {
        public abstract object? Eval(IAttributeContext ctx);
    }
}
