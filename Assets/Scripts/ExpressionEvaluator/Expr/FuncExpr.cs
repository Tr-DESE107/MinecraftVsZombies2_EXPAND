#nullable enable

using System;
using System.Collections.Generic;

namespace ExpressionEvaluator
{
    class FuncExpr : Expr
    {
        private readonly string _name;
        private readonly List<Expr> _args;

        public FuncExpr(string name, List<Expr> args)
        {
            _name = name;
            _args = args;
        }

        public override object? Eval(IAttributeContext ctx)
        {
            var argArray = new object?[_args.Count];
            for (int i = 0; i < argArray.Length; i++)
            {
                argArray[i] = _args[i].Eval(ctx);
            }
            return ctx.EvaluateFunction(_name, argArray);
        }
    }
}
