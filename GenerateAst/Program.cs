﻿namespace GenerateAst
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
            
            DefineAst(outDir, "Expr", new List<string>
            {
                "Binary      : Expr left, Token op, Expr right",
                "Grouping    : Expr expression",
                "Literal     : object value",
                "Unary       : Token op, Expr right",
                "Conditional : Expr left, Expr thenBranch, Expr elseBranch",
                "Variable    : Token name"
            });

            DefineAst(outDir, "Stmt", new List<string>
            {
                "Expression : Expr expr",
                "Print      : Expr expr",
                "Var        : Token name, Expr initialiser"
            });
        }


        private static void DefineAst(string output, string baseName, List<string> types)
        {
            var basePath = $"{output}/{baseName}.cs";

            // Write base class
            using (StreamWriter writer = new StreamWriter(basePath))
            {
                if (types.Any(t => t.Contains("Token ")))
                {
                    writer.WriteLine("using Lox.Tokens;");
                    writer.WriteLine("");
                }

                writer.WriteLine("namespace Lox");
                writer.WriteLine("{");
                writer.WriteLine($"    public abstract class {baseName}");
                writer.WriteLine("    {");
                writer.WriteLine($"        public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
                writer.WriteLine("    }");
                writer.WriteLine("");

                DefineVisitor(writer, baseName, types);

                // Write derived classes
                foreach (var type in types)
                {
                    var className = type.Split(":")[0].Trim();
                    var fields = type.Split(":")[1].Trim();

                    DefineType(writer, baseName, className, fields);
                }

                writer.WriteLine("}");
            }

        }


        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine("");
            writer.WriteLine($"    public class {className} : {baseName}");
            writer.WriteLine("    {");

            var fields = fieldList.Split(", ");

            // Fields
            foreach (var field in fields)
            {
                var typeName = field.Split(" ")[0];
                var fieldName = field.Split(" ")[1];
                writer.WriteLine($"        public {typeName} {char.ToUpper(fieldName[0])}{fieldName.Substring(1)} {{ get; init; }}");
            }

            writer.WriteLine("");

            writer.WriteLine($"        public override T Accept<T>(I{baseName}Visitor<T> visitor)");
            writer.WriteLine("        {");
            writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("        }");

            writer.WriteLine("    }");
        }
        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine($"    public interface I{baseName}Visitor<T>");
            writer.WriteLine("    {");
            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();
                writer.WriteLine($"        T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }
            writer.WriteLine("    }");
        }
    }

}