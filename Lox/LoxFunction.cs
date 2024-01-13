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

            for(var i = 0; i < declaration.Params.Count; i++)
            {
                environment.Define(declaration.Params[i].Lexeme, args[i]);
            }

            InterpreterHelpers.ExecuteBlock(interpreter, declaration.Body, environment);
            return null;
        }

        public override string ToString()
        {
            return $"<function {declaration.Name.Lexeme}>";
        }
    }
}
