#nullable enable

namespace ExpressionEvaluator
{
    class VarExpr : Expr
    {
        private readonly string _name;
        public VarExpr(string name) => _name = name;
        public override object? Eval(IAttributeContext ctx) => ctx.EvaluateVariable(_name);
    }
}
