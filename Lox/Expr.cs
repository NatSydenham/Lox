using Lox.Tokens;

namespace Lox
{
    internal abstract class Expr
    {
        internal abstract T Accept<T>(IVisitor<T> visitor);
    }
    internal interface IVisitor<T>
    {
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
    }

    internal class Binary : Expr
    {
        public Expr Left { get; }
        public Token Op { get; }
        public Expr Right { get; }

        public Binary(Expr left, Token op, Expr right)
        {
            this.Left = left;
            this.Op = op;
            this.Right = right;
        }

        internal override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    internal class Grouping : Expr
    {
        public Expr Expression { get; }

        public Grouping(Expr expression)
        {
            this.Expression = expression;
        }

        internal override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    internal class Literal : Expr
    {
        public object Value { get; }

        public Literal(object value)
        {
            this.Value = value;
        }

        internal override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    internal class Unary : Expr
    {
        public Token Op { get; }
        public Expr Right { get; }

        public Unary(Token op, Expr right)
        {
            this.Op = op;
            this.Right = right;
        }

        internal override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
