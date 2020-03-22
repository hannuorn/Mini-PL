using System;

namespace Mini_PL
{
    class Program
    {
        static private void WriteInstructions()
        {
            Console.WriteLine("\nInstructions:\n");
            Console.WriteLine("   Mini-PL [-AST] filename");
        }

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                Scanner scanner = new Scanner();
                bool ok = scanner.ReadSource(args[0]);
                if (ok)
                {
                    Parser parser = new Parser(scanner);
                    parser.Parse();

                    TypeCheckVisitor typeCheckVisitor = new TypeCheckVisitor(scanner);
                    parser.Get_program().Accept(typeCheckVisitor);

                    if (!scanner.ErrorsFound)
                    {
                        ExecuteVisitor executeVisitor = new ExecuteVisitor(scanner);
                        parser.Get_program().Accept(executeVisitor);
                    }
                } else
                {
                    Console.WriteLine("Cannot read file '" + args[0] + "'.");
                }

            } else if (args.Length == 2)
            {
                if (args[0].Equals("-AST"))
                {
                    Scanner scanner = new Scanner();
                    bool ok = scanner.ReadSource(args[1]);
                    if (ok)
                    {
                        Parser parser = new Parser(scanner);
                        parser.Parse();
                        TypeCheckVisitor typeCheckVisitor = new TypeCheckVisitor(scanner);
                        parser.Get_program().Accept(typeCheckVisitor);
                        PrintVisitor printVisitor = new PrintVisitor();
                        parser.Get_program().Accept(printVisitor);
                    } else
                    {
                        Console.WriteLine("Cannot read file '" + args[1] + "'.");
                    }
                } else
                {
                    WriteInstructions();
                }
            } else
            {
                WriteInstructions();
            }
        }
    }
}
