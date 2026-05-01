#nullable enable

namespace ExpressionEvaluator
{
    class StringExpr : Expr
    {
        private readonly string _value;
        public StringExpr(string v) => _value = v;
        public override object? Eval(IAttributeContext ctx) => _value;
    }
}
