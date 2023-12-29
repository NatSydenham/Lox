using Lox.Tokens;

namespace Lox
{
    public class Environment
    {
        private readonly Dictionary<string, object> values = new();

        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out object? value))
            {
                return value;
            }

            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
        }
    }
}
