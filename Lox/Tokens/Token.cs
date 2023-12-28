namespace Lox.Tokens
{
    public class Token
    {
        public TokenType Type { get; init; }
        public string Lexeme { get; init; } = string.Empty;
        public object? Literal { get; init; }
        public int Line { get; init; }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
