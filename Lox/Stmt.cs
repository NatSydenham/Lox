using Lox.Tokens;

namespace Lox
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    public interface IStmtVisitor<T>
    {
        T VisitExpressionStmt(Expression stmt);
        T VisitPrintStmt(Print stmt);
        T VisitVarStmt(Var stmt);
    }

    public class Expression : Stmt
    {
        public Expr Expr { get; init; }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
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
