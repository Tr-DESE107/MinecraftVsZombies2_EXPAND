#nullable enable

namespace ExpressionEvaluator
{
    class NumberExpr : Expr
    {
        private readonly double _value;
        public NumberExpr(double v) => _value = v;
        public override object? Eval(IAttributeContext ctx) => _value;
    }
}
