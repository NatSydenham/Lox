/*
*    This file has been autogenerated by GenerateAst
*    Do not edit this file directly.
*/

using Lox.Tokens;

namespace Lox
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    public interface IExprVisitor<T>
    {
        T VisitAssignExpr(Assign expr);
        T VisitBinaryExpr(Binary expr);
        T VisitCallExpr(Call expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitLogicalExpr(Logical expr);
        T VisitUnaryExpr(Unary expr);
        T VisitConditionalExpr(Conditional expr);
        T VisitVariableExpr(Variable expr);
    }

    public class Assign : Expr
    {
        public Token Name { get; init; }
        public Expr Value { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }

    public class Binary : Expr
    {
        public Expr Left { get; init; }
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Call : Expr
    {
        public Expr Callee { get; init; }
        public Token Paren { get; init; }
        public List<Expr> Arguments { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitCallExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public object Value { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    public class Logical : Expr
    {
        public Expr Left { get; init; }
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    public class Conditional : Expr
    {
        public Expr Left { get; init; }
        public Expr ThenBranch { get; init; }
        public Expr ElseBranch { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitConditionalExpr(this);
        }
    }

    public class Variable : Expr
    {
        public Token Name { get; init; }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}
