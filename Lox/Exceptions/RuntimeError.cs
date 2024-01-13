using Lox.Tokens;

namespace Lox
{
    public class RuntimeError : Exception
    {
        public readonly Token token;
        public RuntimeError(Token token, string message) : base(message) { this.token = token; }

    }
}
