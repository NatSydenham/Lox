using System.Text;

namespace Lox
{
    internal class ASTPrinter : IVisitor<string>
    {
        public string VisitBinaryExpr(Binary expr)
            => Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);

        public string VisitGroupingExpr(Grouping expr)
            => Parenthesize("group", expr.Expression);

        public string VisitLiteralExpr(Literal expr)
            => expr.Value is null ? "nil" : (expr.Value.ToString() ?? string.Empty);

        public string VisitUnaryExpr(Unary expr)
            => Parenthesize(expr.Op.Lexeme, expr.Right);

        public string VisitConditionalExpr(Conditional expr)
            => Parenthesize("conditional", expr.Left, expr.ThenBranch, expr.ElseBranch);

        public string Print(Expr expr)
            => expr.Accept(this);

        // Recursively print expression tree
        private string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();

            builder.Append("(").Append(name);

            foreach(var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
