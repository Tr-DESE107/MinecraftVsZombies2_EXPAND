#nullable enable

using System;
using System.Collections.Generic;


namespace ExpressionEvaluator
{
    class Parser
    {
        private Tokenizer? _tk;
        private Token _cur;

        public Expr Parse(string text)
        {
            _tk = new Tokenizer(text);
            _cur = _tk.Next();
            return ParseExpr();
        }

        private void Next()
        {
            if (_tk == null)
                throw new NullReferenceException("Parser is not parsing text!");
            _cur = _tk.Next();
        }

        private Expr ParseExpr()
        {
            Expr left = ParseTerm();
            while (_cur.Type == TokenType.Plus || _cur.Type == TokenType.Minus)
            {
                var op = _cur.Type;
                Next();
                var right = ParseTerm();
                left = new BinaryExpr(left, right, op);
            }
            return left;
        }

        private Expr ParseTerm()
        {
            Expr left = ParseFactor();
            while (_cur.Type == TokenType.Mul || _cur.Type == TokenType.Div)
            {
                var op = _cur.Type;
                Next();
                var right = ParseFactor();
                left = new BinaryExpr(left, right, op);
            }
            return left;
        }

        private Expr ParseFactor()
        {
            if (_cur.Type == TokenType.Number)
            {
                var n = new NumberExpr(_cur.Number);
                Next();
                return n;
            }

            if (_cur.Type == TokenType.String)
            {
                var s = new StringExpr(_cur.Text);
                Next();
                return s;
            }

            if (_cur.Type == TokenType.Identifier)
            {
                string name = _cur.Text;
                Next();

                if (_cur.Type == TokenType.LParen)
                {
                    Next();
                    var args = new List<Expr>();

                    if (_cur.Type != TokenType.RParen)
                    {
                        while (true)
                        {
                            args.Add(ParseExpr());
                            if (_cur.Type == TokenType.Comma)
                            {
                                Next();
                                continue;
                            }
                            break;
                        }
                    }

                    if (_cur.Type != TokenType.RParen)
                        throw new Exception("Missing )");

                    Next();
                    return new FuncExpr(name, args);
                }

                return new VarExpr(name);
            }

            if (_cur.Type == TokenType.LParen)
            {
                Next();
                var e = ParseExpr();
                if (_cur.Type != TokenType.RParen)
                    throw new Exception("Missing )");
                Next();
                return e;
            }

            throw new Exception("Unexpected token");
        }
    }
}
