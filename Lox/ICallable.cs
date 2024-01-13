namespace Lox
{
    public interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> args);
    }
}
