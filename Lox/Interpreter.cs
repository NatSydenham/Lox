using Lox.Tokens;

namespace Lox
{
    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        private readonly Environment env = new();

        public object VisitBinaryExpr(Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string || right is string)
                    {
                        return $"{left}{right}";
                    }
                    throw new RuntimeError(expr.Op, "Operands must be two numbers or at least one string");
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Op, left, right);
                    if ((double) right == 0)
                    {
                        throw new RuntimeError(expr.Op, "Cannot divide by zero");
                    }
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
            }

            return null;
        }

        public object VisitConditionalExpr(Conditional expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Grouping expr) => Evaluate(expr.Expression);

        public object VisitLiteralExpr(Literal expr) => expr.Value;
        public object VisitUnaryExpr(Unary expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Op, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
            }

            throw new RuntimeError(expr.Op, $"Unexpected parsing of {expr.Op.Type} as Unary");
        }

        public object VisitVariableExpr(Variable expr)
        {
            return env.Get(expr.Name);
        }


        public object VisitExpressionStmt(Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        public object VisitPrintStmt(Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }
        public object VisitVarStmt(Var stmt)
        {
            object value = null;
            if (stmt.Initialiser is not null)
            {
                value = Evaluate(stmt.Initialiser);
            }

            env.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch(RuntimeError err)
            {
                Program.RuntimeError(err);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private string Stringify(object? value)
        {
            if (value is null)
            {
                return "nil";
            }
            
            if (value is double)
            {
                var text = value.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return value.ToString()!;
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object? left, object? right)
        {
            if (left is double && right is double)
            {
                return;
            }

            throw new RuntimeError(op, "Operands must be numbers");
        }

        private object Evaluate(Expr expression) => expression.Accept(this);

        // nil or false is falsy, everything else is truthy.
        private bool IsTruthy(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool)
            {
                return (bool)obj;
            }

            return true;
        }

        private bool IsEqual(object? left, object? right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null) return false;

            return left.Equals(right);
        }


    }
}
