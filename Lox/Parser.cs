using Lox.Tokens;
using static Lox.Tokens.TokenType;

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
                if (Match(VAR))
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
            if (Match(PRINT))
            {
                return PrintStatement();
            }
            if (Match(WHILE))
            {
                return WhileStatement();
            }
            if (Match(LEFT_BRACE))
            {
                return new Block { Statements = BlockStatement() };
            }
            if (Match(IF))
            {
                return IfStatement();
            }

            return ExpressionStatement();
        }

        private Stmt WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' before condition");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition");

            var body = Statement();

            return new While { Expr = condition, Body = body };
        }

        private Stmt IfStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' before condition.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition.");
            var thenBranch = Statement();
            Stmt elseBranch = null;

            if (Match(ELSE))
            {
                elseBranch = Statement();
            }

            return new If { Expr = condition, ThenBranch = thenBranch, ElseBranch = elseBranch };
        }

        private List<Stmt> BlockStatement()
        {
            var statements = new List<Stmt>();
            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                // Declaration has lowest precedence, so start evaluating statements again as lowest precedence.
                statements.Add(Declaration());
            }

            Consume(RIGHT_BRACE, "Expect '}' at end of block");

            return statements;
        }

        private Stmt ExpressionStatement()
        {
            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after expression");
            return new Expression { Expr = value };
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after value");
            return new Print { Expr = value };
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name.");
            Expr initialiser = null;

            if (Match(EQUAL))
            {
                initialiser = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after variable declaration.");
            return new Var { Initialiser = initialiser, Name = name };
        }

        private Expr Expression()
        {
            return Comma();
        }
        private Expr Comma()
        {
            var expr = Assignment();
            while (Match(COMMA))
            {
                var op = Previous();
                var right = Assignment();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }

            return expr;
        }


        private Expr Assignment()
        {
            var expr = Or();

            if (Match(EQUAL))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Variable)
                {
                    var name = ((Variable)expr).Name;
                    return new Assign { Name = name, Value = value };
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(OR))
            {
                var op = Previous();
                var right = And();
                
                expr = new Logical { Left = expr, Op = op, Right = right };
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Conditional();

            while (Match(AND))
            {
                var op = Previous();
                var right = Conditional();

                expr = new Logical { Left = expr, Op = op, Right = right };
            }

            return expr;
        }

        private Expr Conditional()
        {
            var expr = Equality();
            if (Match(QUESTION))
            {
                var then = Expression();
                Consume(COLON, "Expect ':' after then branch of conditional");
                var elseBranch = Conditional();
                expr = new Conditional { Left = expr, ThenBranch = then, ElseBranch = elseBranch };
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();
            while (Match(BANG_EQUAL, EQUAL_EQUAL))
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
            while (Match(GREATER, GREATER_EQUAL, LESS_EQUAL, LESS))
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
            while (Match(MINUS, PLUS))
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
            while (Match(SLASH, STAR))
            {
                var op = Previous();
                var right = Unary();
                expr = new Binary { Left = expr, Op = op, Right = right };
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                return new Unary { Op = Previous(), Right = Unary() };
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(FALSE))
            {
                return new Literal { Value = false };
            }

            if (Match(TRUE))
            {
                return new Literal { Value = true };
            }

            if (Match(NIL))
            {
                return new Literal { Value = null };
            }

            if (Match(NUMBER, STRING))
            {
                return new Literal { Value = Previous().Literal };
            }

            if (Match(IDENTIFIER))
            {
                return new Variable { Name = Previous() };
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping { Expression = expr };
            }


            if (Match(BANG, BANG_EQUAL))
            {
                Error(Previous(), "Missing left hand operand");
                Equality();
                return null;
            }

            if (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                Error(Previous(), "Missing left hand operand");
                Comparison();
                return null;
            }

            if (Match(PLUS))
            {
                Error(Previous(), "Missing left hand operand");
                Term();
                return null;
            }

            if (Match(STAR, SLASH))
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
            => Peek().Type == EOF;

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
                if (Previous().Type == SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}
