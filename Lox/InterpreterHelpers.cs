using Lox.Tokens;

namespace Lox
{
    public static class InterpreterHelpers
    {
        public static string Stringify(object? value)
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

        public static void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeError(op, "Operand must be a number.");
        }

        public static void CheckNumberOperands(Token op, object? left, object? right)
        {
            if (left is double && right is double)
            {
                return;
            }

            throw new RuntimeError(op, "Operands must be numbers");
        }

        public static object Evaluate(this Interpreter interpreter, Expr expression) => expression.Accept(interpreter);
        public static void Execute(this Interpreter interpreter, Stmt stmt)
        {
            stmt.Accept(interpreter);
        }

        // nil or false is falsy, everything else is truthy.
        public static bool IsTruthy(object? obj)
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

        public static bool IsEqual(object? left, object? right)
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
