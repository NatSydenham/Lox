namespace Lox.NativeFunctions
{
    public class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> args)
        {
            return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
        }

        public override string ToString()
        {
            return "<native function>";
        }
    }
}

