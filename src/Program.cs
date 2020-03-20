using System;


namespace Mini_PL
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                Scanner scanner = new Scanner();
                scanner.ReadSource(args[0]);
                /*
                Token token;
                do
                {
                    token = scanner.GetNextToken();
                    Console.WriteLine("kind: " + token.Kind.ToString() + ", \t\tlexeme: " + token.Lexeme);
                } while (token.Kind != TokenKind.EndOfSource);
                scanner.Reset();
                */

                Parser parser = new Parser(scanner);
                parser.Parse();

                TypeCheckVisitor typeCheckVisitor = new TypeCheckVisitor(scanner);
                parser.Get_program().Accept(typeCheckVisitor);

                PrintVisitor printVisitor = new PrintVisitor();
                // parser.Get_program().Accept(printVisitor);

                if (!scanner.ErrorsFound) 
                {
                    ExecuteVisitor executeVisitor = new ExecuteVisitor(scanner);
                    parser.Get_program().Accept(executeVisitor);
                }
            } else if (args.Length == 2)
            {
                if (args[0].Equals("-AST"))
                {
                    Scanner scanner = new Scanner();
                    scanner.ReadSource(args[1]);
                    Parser parser = new Parser(scanner);
                    parser.Parse();
                    TypeCheckVisitor typeCheckVisitor = new TypeCheckVisitor(scanner);
                    parser.Get_program().Accept(typeCheckVisitor);
                    PrintVisitor printVisitor = new PrintVisitor();
                    parser.Get_program().Accept(printVisitor);
                }
            }
        }
    }
}
