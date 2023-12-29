using Lox.Tokens;
using System.Text;

namespace Lox
{
    public class Program
    {
        private const int ARGS_LENGTH = 1;
        private const int INCORRECT_USAGE = 64;
        private const int ERROR = 65;
        private const int RUNTIME_ERROR = 70;

        private static readonly Interpreter interpreter = new Interpreter();

        private static bool hadError = false;
        private static bool hadRuntimeError = false;
        public static void Main(string[] args)
        {
            if (args.Length > ARGS_LENGTH)
            {
                Console.WriteLine("Usage: dotnet lox.dll [script]");
                Environment.Exit(INCORRECT_USAGE);
            }
            else if (args.Length == ARGS_LENGTH)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string file)
        {
            var bytes = File.ReadAllBytes(file);
            Run(Encoding.UTF8.GetString(bytes));
            if (hadError)
            {
                Environment.Exit(ERROR);
            }
            if (hadRuntimeError)
            {
                Environment.Exit(RUNTIME_ERROR);
            }
        }

        private static void RunPrompt()
        {
            using (var reader = new StreamReader(Console.OpenStandardInput()))
            {
                for (; ; )
                {
                    Console.WriteLine("> ");
                    var line = reader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    Run(line);
                    hadError = false;
                }
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            if (hadError)
            {
                return;
            }

            interpreter.Interpret(tree);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.token.Line}]");
            hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            hadError = true;
        }
    }
}