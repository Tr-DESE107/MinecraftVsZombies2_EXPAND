#nullable enable

using System;

namespace ExpressionEvaluator
{
    class BinaryExpr : Expr
    {
        private readonly Expr _left, _right;
        private readonly TokenType _op;

        public BinaryExpr(Expr l, Expr r, TokenType op)
        {
            _left = l;
            _right = r;
            _op = op;
        }

        public override object? Eval(IAttributeContext ctx)
        {
            double a = Convert.ToDouble(_left.Eval(ctx));
            double b = Convert.ToDouble(_right.Eval(ctx));
            switch (_op)
            {
                case TokenType.Plus: return a + b;
                case TokenType.Minus: return a - b;
                case TokenType.Mul: return a * b;
                case TokenType.Div: return b == 0 ? 0 : a / b;
                default: throw new Exception("Invalid op");
            }
        }
    }
}
