using Lox.Exceptions;

namespace Lox
{
    public class LoxFunction : ICallable
    {
        private readonly Function declaration;

        public LoxFunction(Function declaration)
        {
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.Params.Count;
        }

        public object Call(Interpreter interpreter, List<object> args)
        {
            var environment = new Environment(interpreter.globals);

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
