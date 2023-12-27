namespace GenerateAst
{
    public class Program
    {
        private const int ARGS_LENGTH = 1;
        private const int INCORRECT_USAGE = 64;
        static void Main(string[] args)
        {
            if (args.Length != ARGS_LENGTH)
            {
                Console.WriteLine("Usage: dotnet generateast.dll [output_dir]");
                Environment.Exit(INCORRECT_USAGE);
            }

            var outDir = args[0];
            var types = new List<string>
            {
                "Binary   : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Unary    : Token op, Expr right"
            };

            DefineAst(outDir, "Expr", types);
        }


        private static void DefineAst(string output, string baseName, List<string> types)
        {
            var basePath = $"{output}/{baseName}.cs";

            // Write base class
            using (StreamWriter outFile = new StreamWriter(basePath))
            {
                if (types.Any(t => t.Contains("Token ")))
                {
                    outFile.WriteLine("using Lox.Tokens;");
                    outFile.WriteLine("");
                }

                outFile.WriteLine("namespace Lox");
                outFile.WriteLine("{");
                outFile.WriteLine($"    internal abstract class {baseName}");
                outFile.WriteLine("    {");
                outFile.WriteLine("    }");


                // Write derived classes
                foreach (var type in types)
                {
                    var className = type.Split(":")[0].Trim();
                    var fields = type.Split(":")[1].Trim();

                    DefineType(outFile, baseName, className, fields);
                }

                outFile.WriteLine("}");
            }

        }


        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine("");
            writer.WriteLine($"    internal class {className} : {baseName}");
            writer.WriteLine("    {");

            var fields = fieldList.Split(", ");

            // Fields
            foreach (var field in fields)
            {
                writer.WriteLine($"        private readonly {field};");
            }

            writer.WriteLine("");

            // Constructor
            writer.WriteLine($"        public {className}({fieldList})");
            writer.WriteLine("        {");

            foreach(var field in fields)
            {
                var name = field.Split(" ")[1];
                writer.WriteLine($"            this.{name} = {name};");
            }

            writer.WriteLine("        }");

            writer.WriteLine("    }");
        }
    }
}