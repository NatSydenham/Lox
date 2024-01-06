using Lox.Tokens;

namespace Lox
{
    public class Environment
    {
        private readonly Dictionary<string, object> values = new();
        private readonly Environment enclosing;

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (enclosing is not null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
        }

        public object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out object? value))
            {
                return value;
            }

            if (enclosing is not null)
            {
                return enclosing.Get(name);
            }


            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
        }
    }
}
