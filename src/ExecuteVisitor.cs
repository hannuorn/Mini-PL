using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mini_PL
{
    class ExecuteVisitor : ASTVisitor
    {
        private Scanner scanner;
        private EvaluateVisitor evaluateVisitor;

        public ExecuteVisitor(Scanner scanner)
        {
            this.scanner = scanner;
            evaluateVisitor = new EvaluateVisitor();
        }

        public override void Visit(AST_assert_statement assert_statement)
        {
            assert_statement.Expression.Accept(evaluateVisitor);
            bool assertion = evaluateVisitor.Value.BoolValue;
            if (assertion == false)
            {
                scanner.Error("Assertion error.", 
                    assert_statement.Expression.Row, 
                    assert_statement.Expression.Column);
            }
        }

        public override void Visit(AST_assignment assignment)
        {
            assignment.Accept(evaluateVisitor);
        }

        public override void Visit(AST_for_statement for_statement)
        {
            for_statement.From.Accept(evaluateVisitor);
            Variable from = evaluateVisitor.Value.Copy();
            for_statement.To.Accept(evaluateVisitor);
            Variable to = evaluateVisitor.Value.Copy();
            int a = from.IntValue;
            int b = to.IntValue;
            evaluateVisitor.SetVariable(for_statement.Identifier.Name, a);
            for (int i = a; i <= b; i++)
            {
                for_statement.StatementList.Accept(this);
                evaluateVisitor.IncrementVariable(for_statement.Identifier);
            }
        }

        override public void Visit(AST_print_statement print_statement)
        {
            print_statement.Expression.Accept(evaluateVisitor);
            Console.WriteLine(evaluateVisitor.Value.ToString());
        }

        public override void Visit(AST_read_statement read_statement)
        {
            bool ok = false;
            while (!ok)
            {
                string input = Console.ReadLine();
                if (evaluateVisitor.SetVariable(read_statement.Identifier.Name, input))
                {
                    break;
                }
                Console.WriteLine("Not a valid integer.");
            }
        }

        override public void Visit(AST_variable_declaration variable_declaration)
        {
            variable_declaration.Accept(evaluateVisitor);
        }

    }
}
