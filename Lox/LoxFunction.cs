using Lox.Exceptions;

namespace Lox
{
    public class LoxFunction : ICallable
    {
        private readonly Function declaration;
        private readonly Environment closure;

        public LoxFunction(Function declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }

        public int Arity()
        {
            return declaration.Params.Count;
        }

        public object Call(Interpreter interpreter, List<object> args)
        {
            var environment = new Environment(closure);

            for (var i = 0; i < declaration.Params.Count; i++)
            {
                environment.Define(declaration.Params[i].Lexeme, args[i]);
            }

            try
            {
                InterpreterHelpers.ExecuteBlock(interpreter, declaration.Body, environment);
            }
            catch (ReturnValue returnValue)
            {
                return returnValue.returnValue;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<function {declaration.Name.Lexeme}>";
        }
    }
}
