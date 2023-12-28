using System.Text;

namespace Lox
{
    internal class ASTPrinter : IVisitor<string>
    {
        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            throw new NotImplementedException();
        }

        public string VisitLiteralExpr(Literal expr)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            throw new NotImplementedException();
        }

        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }


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
