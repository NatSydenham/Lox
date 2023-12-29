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
}
