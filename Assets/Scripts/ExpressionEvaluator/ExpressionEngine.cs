#nullable enable

using System.Collections.Generic;

// =============================
// IL2CPP SAFE EXPRESSION ENGINE
// - No Reflection.Emit
// - No Expression.Compile
// - Pure interpreter (AST)
// - Designed for Unity + Attribute Dictionary systems
// =============================

namespace ExpressionEvaluator
{
    public static class ExpressionEngine
    {
        private static readonly Parser _parser = new Parser();
        private static readonly Dictionary<string, CompiledExpression> _cache = new();

        public static CompiledExpression Compile(string expr)
        {
            if (_cache.TryGetValue(expr, out var c))
                return c;

            var parsed = _parser.Parse(expr);
            var compiled = new CompiledExpression(parsed, expr);
            _cache[expr] = compiled;
            return compiled;
        }
    }
}

// =============================
// Example Usage (Unity side)
// =============================

/*
class AttrContext : IAttributeContext
{
    private Dictionary<string, double> _data;

    public AttrContext(Dictionary<string, double> data)
    {
        _data = data;
    }

    public double Get(string name)
    {
        return _data.TryGetValue(name, out var v) ? v : 0;
    }
}

// Usage:
var expr = ExpressionEngine.Compile("atk * 2 + max(hp, 10)");
var result = expr.Evaluate(new AttrContext(new Dictionary<string,double>
{
    {"atk", 5},
    {"hp", 8}
}));
*/