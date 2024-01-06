using static Lox.InterpreterHelpers;
using static Lox.Tokens.TokenType;

namespace Lox
{
    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        private Environment env = new(null);

        private static object uninitialised = new object();

        public object VisitBinaryExpr(Binary expr)
        {
            var left = InterpreterHelpers.Evaluate(this, expr.Left);
            var right = InterpreterHelpers.Evaluate(this, expr.Right);

            switch (expr.Op.Type)
            {
                case PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string || right is string)
                    {
                        return $"{left}{right}";
                    }
                    throw new RuntimeError(expr.Op, "Operands must be two numbers or at least one string");
                case MINUS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left - (double)right;
                case COMMA:
                    return right;
                case SLASH:
                    CheckNumberOperands(expr.Op, left, right);
                    if ((double)right == 0)
                    {
                        throw new RuntimeError(expr.Op, "Cannot divide by zero");
                    }
                    return (double)left / (double)right;
                case STAR:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case LESS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case GREATER:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case LESS_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                case EQUAL_EQUAL:
                    return IsEqual(left, right);
                case BANG_EQUAL:
                    return !IsEqual(left, right);
            }

            return null;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            var left = InterpreterHelpers.Evaluate(this, expr.Left);
            if (expr.Op.Type == OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return InterpreterHelpers.Evaluate(this, expr.Right);
        }

        public object VisitConditionalExpr(Conditional expr) =>
            IsTruthy(InterpreterHelpers.Evaluate(this, expr.Left)) ?
                InterpreterHelpers.Evaluate(this, expr.ThenBranch) :
                InterpreterHelpers.Evaluate(this, expr.ElseBranch);
        public object VisitGroupingExpr(Grouping expr) 
            => InterpreterHelpers.Evaluate(this, expr.Expression);

        public object VisitLiteralExpr(Literal expr) 
            => expr.Value;
        public object VisitUnaryExpr(Unary expr)
        {
            var right = InterpreterHelpers.Evaluate(this, expr.Right);

            switch (expr.Op.Type)
            {
                case MINUS:
                    CheckNumberOperand(expr.Op, right);
                    return -(double)right;
                case BANG:
                    return !IsTruthy(right);
            }

            throw new RuntimeError(expr.Op, $"Unexpected parsing of {expr.Op.Type} as Unary");
        }

        public object VisitAssignExpr(Assign expr)
        {
            var value = InterpreterHelpers.Evaluate(this, expr.Value);
            env.Assign(expr.Name, value);
            return value;
        }

        public object VisitVariableExpr(Variable expr)
        {
            var value = env.Get(expr.Name);

            if (value == uninitialised)
            {
                throw new RuntimeError(expr.Name, "Must initialise variable before use");
            }

            return value;
        }

        public object VisitExpressionStmt(Expression stmt)
        {
            InterpreterHelpers.Evaluate(this, stmt.Expr);
            return null;
        }

        public object VisitBlockStmt(Block block)
        {
            var previous = this.env;
            try
            {
                this.env = new Environment(this.env);

                foreach (var stmt in block.Statements)
                {
                    InterpreterHelpers.Execute(this, stmt);
                }
            } 
            finally
            {
                this.env = previous;
            }

            return null;
        }

        public object VisitPrintStmt(Print stmt)
        {
            var value = InterpreterHelpers.Evaluate(this, stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }
        public object VisitVarStmt(Var stmt)
        {
            object value = uninitialised;

            if (stmt.Initialiser is not null)
            {
                value = InterpreterHelpers.Evaluate(this, stmt.Initialiser);
            }

            env.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public object VisitWhileStmt(While stmt)
        {
            while (IsTruthy(InterpreterHelpers.Evaluate(this, stmt.Expr)))
            {
                InterpreterHelpers.Execute(this, stmt.Body);
            }

            return null;
        }

        public object VisitIfStmt(If stmt)
        {
            if (IsTruthy(InterpreterHelpers.Evaluate(this, stmt.Expr)))
            {
                InterpreterHelpers.Execute(this, stmt.ThenBranch);

            }
            else if (stmt.ElseBranch is not null)
            {
                InterpreterHelpers.Execute(this, stmt.ElseBranch);
            }

            return null;    
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var stmt in statements)
                {
                    InterpreterHelpers.Execute(this, stmt);
                }
            }
            catch (RuntimeError err)
            {
                Program.RuntimeError(err);
            }
        }
    }
}
