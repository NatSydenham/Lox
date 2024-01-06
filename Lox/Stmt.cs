/*
*    This file has been autogenerated by GenerateAst
*    Do not edit this file directly.
*/

using Lox.Tokens;

namespace Lox
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    public interface IStmtVisitor<T>
    {
        T VisitBlockStmt(Block stmt);
        T VisitExpressionStmt(Expression stmt);
        T VisitIfStmt(If stmt);
        T VisitPrintStmt(Print stmt);
        T VisitWhileStmt(While stmt);
        T VisitVarStmt(Var stmt);
    }

    public class Block : Stmt
    {
        public List<Stmt> Statements { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    public class Expression : Stmt
    {
        public Expr Expr { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public class If : Stmt
    {
        public Expr Expr { get; init; }
        public Stmt ThenBranch { get; init; }
        public Stmt ElseBranch { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }

    public class Print : Stmt
    {
        public Expr Expr { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public class While : Stmt
    {
        public Expr Expr { get; init; }
        public Stmt Body { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }

    public class Var : Stmt
    {
        public Token Name { get; init; }
        public Expr Initialiser { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }
}
