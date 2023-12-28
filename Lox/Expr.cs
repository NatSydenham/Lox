using Lox.Tokens;

namespace Lox
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }

    public interface IVisitor<T>
    {
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
        T VisitConditionalExpr(Conditional expr);
    }

    public class Binary : Expr
    {
        public Expr Left { get; init; }
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; init; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public object Value { get; init; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    public class Conditional : Expr
    {
        public Expr Left { get; init; }
        public Expr ThenBranch { get; init; }
        public Expr ElseBranch { get; init; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitConditionalExpr(this);
        }
    }
}
