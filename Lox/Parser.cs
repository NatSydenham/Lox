using Lox.Tokens;

namespace Lox
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR))
                {
                    return VarDeclaration();
                }

                return Statement();

            }

            catch (ParseError e)
            {
                Synchronise();
                return null;
            }
        }

        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) {
                return PrintStatement();
            }

            return ExpressionStatement();
        }

        private Stmt ExpressionStatement()
        {
            var value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression");
            return new Expression { Expr = value };
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value");
            return new Print { Expr = value };
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");
            Expr initialiser = null;

            if (Match(TokenType.EQUAL))
            {
                initialiser = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Var { Initialiser = initialiser, Name = name };
        }

        private Expr Expression()
        {
            return Comma();
        }

        private Expr Comma()
        {
            var expr = Conditional();
            while (Match(TokenType.COMMA))
            {
                var op = Previous();
                var right = Conditional();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }

            return expr;
        }

        private Expr Conditional()
        {
            var expr = Equality();
            if (Match(TokenType.QUESTION))
            {
                var then = Expression();
                Consume(TokenType.COLON, "Expect ':' after then branch of conditional");
                var elseBranch = Conditional();
                expr = new Conditional { Left = expr, ThenBranch = then, ElseBranch = elseBranch };
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Term();
            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS_EQUAL, TokenType.LESS))
            {
                var op = Previous();
                var right = Term();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }

            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                var op = Previous();
                var right = Factor();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }

            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                var op = Previous();
                var right = Unary();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                return new Unary { Op = Previous(), Right = Unary() };
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE))
            {
                return new Literal { Value = false };
            }

            if (Match(TokenType.TRUE))
            {
                return new Literal { Value = true };
            }

            if (Match(TokenType.NIL))
            {
                return new Literal { Value = null };
            }

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal { Value = Previous().Literal };
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable { Name = Previous() };
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                var expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping { Expression = expr };
            }


            if (Match(TokenType.BANG, TokenType.BANG_EQUAL))
            {
                Error(Previous(), "Missing left hand operand");
                Equality();
                return null;
            }

            if (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Error(Previous(), "Missing left hand operand");
                Comparison();
                return null;
            }

            if (Match(TokenType.PLUS))
            {
                Error(Previous(), "Missing left hand operand");
                Term();
                return null;
            }

            if (Match(TokenType.STAR, TokenType.SLASH))
            {
                Error(Previous(), "Missing left hand operand");
                Factor();
                return null;
            }



            throw Error(Peek(), "Expect expression.");
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }

            return Previous();
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private bool IsAtEnd()
            => Peek().Type == TokenType.EOF;

        private Token Peek()
            => tokens[current];

        private Token Previous()
            => tokens[current - 1];

        private ParseError Error(Token token, string message)
        {
            Program.Error(token, message);
            return new ParseError();
        }

        private void Synchronise()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}
