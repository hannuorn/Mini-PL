using System;


namespace Mini_PL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Mini-PL\n\n");
            if (args.Length == 1)
            {
                Scanner scanner = new Scanner();
                scanner.ReadSource(args[0]);
                Token token;
                do
                {
                    token = scanner.GetNextToken();
                    Console.WriteLine("kind: " + token.Kind.ToString() + ", \t\tlexeme: " + token.Lexeme);
                } while (token.Kind != TokenKind.EndOfSource);
                scanner.Reset();

                Parser parser = new Parser(scanner);
                parser.Parse();
            }
        }
    }
}
