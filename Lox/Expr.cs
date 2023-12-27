using Lox.Tokens;

namespace Lox
{
    internal abstract class Expr
    {
    }

    internal class Binary : Expr
    {
        private readonly Expr left;
        private readonly Token op;
        private readonly Expr right;

        public Binary(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
    }

    internal class Grouping : Expr
    {
        private readonly Expr expression;

        public Grouping(Expr expression)
        {
            this.expression = expression;
        }
    }

    internal class Literal : Expr
    {
        private readonly object value;

        public Literal(object value)
        {
            this.value = value;
        }
    }

    internal class Unary : Expr
    {
        private readonly Token op;
        private readonly Expr right;

        public Unary(Token op, Expr right)
        {
            this.op = op;
            this.right = right;
        }
    }
}
