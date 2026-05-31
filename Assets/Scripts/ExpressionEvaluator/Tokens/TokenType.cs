#nullable enable

namespace ExpressionEvaluator
{
    enum TokenType
    {
        Number,
        String,
        Identifier,
        Plus, Minus, Mul, Div,
        LParen, RParen,
        Comma,
        End
    }
}