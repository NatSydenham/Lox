using System.Text;

namespace Lox
{
    public class Program
    {
        private const int ARGS_LENGTH = 1;
        private const int INCORRECT_USAGE = 64;

        private static bool hadError = false;
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
                Environment.Exit(INCORRECT_USAGE);
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
            Console.WriteLine(source);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            hadError = true;
        }
    }
}