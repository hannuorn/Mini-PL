using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    abstract class ASTVisitor
    {
        public virtual void Visit(ASTDummy_statement dummy_statement)
        {

        }

        public virtual void Visit(ASTDummy_operand dummy_operand)
        {

        }

        public virtual void Visit(AST_assert_statement assert_statement)
        {
            IncrementDepth();
            assert_statement.Expression.Accept(this);
            DecrementDepth();
        }

        public virtual void Visit(AST_assignment assignment)
        {
            IncrementDepth();
            assignment.Identifier.Accept(this);
            assignment.Expression.Accept(this);
            DecrementDepth();
        }
        public virtual void Visit(AST_binary_operator binary_operator)
        {
            IncrementDepth();
            binary_operator.LeftOperand.Accept(this);
            binary_operator.RightOperand.Accept(this);
            DecrementDepth();
        }
        public virtual void Visit(AST_bool_literal bool_literal)
        {

        }
        public virtual void Visit(AST_expression expression)
        {
        }
        public virtual void Visit(AST_expression_operand expression_operand)
        {
            if (expression_operand.Expression != null)
            {
                IncrementDepth();
                expression_operand.Expression.Accept(this);
                DecrementDepth();
            }
        }

        public virtual void Visit(AST_for_statement for_statement)
        {
            IncrementDepth();
            for_statement.Identifier.Accept(this);
            for_statement.From.Accept(this);
            for_statement.To.Accept(this);
            for_statement.StatementList.Accept(this);
            DecrementDepth();
        }

        public virtual void Visit(AST_identifier identifier)
        {

        }
        public virtual void Visit(AST_integer_literal integer_literal)
        {

        }

        public virtual void Visit(AST_print_statement print_statement)
        {
            IncrementDepth();
            print_statement.Expression.Accept(this);
            DecrementDepth();
        }

        public virtual void Visit(AST_program program)
        {
            IncrementDepth();
            program.StatementList.Accept(this);
            DecrementDepth();
        }

        public virtual void Visit(AST_read_statement read_statement)
        {
            IncrementDepth();
            read_statement.Identifier.Accept(this);
            DecrementDepth();
        }

        public virtual void Visit(AST_statement_list statement_list)
        {
            IncrementDepth();
            foreach (AST_statement statement in statement_list.statement_list)
            {
                if (statement != null)
                {
                    statement.Accept(this);
                }
            }
            DecrementDepth();
        }

        public virtual void Visit(AST_string_literal string_literal)
        {

        }

        public virtual void Visit(AST_type type)
        {

        }

        public virtual void Visit(AST_unary_operator unary_operator)
        {
            IncrementDepth();
            unary_operator.Operand.Accept(this);
            DecrementDepth();
        }

        public virtual void Visit(AST_variable_declaration variable_declaration)
        {
            IncrementDepth();
            variable_declaration.Identifier.Accept(this);
            variable_declaration.Type.Accept(this);
            if (variable_declaration.Expression != null)
            {
                variable_declaration.Expression.Accept(this);
            }
            DecrementDepth();
        }


        private int depth = 0;

        public void IncrementDepth()
        {
            depth++;
        }

        public void DecrementDepth()
        {
            depth--;
        }

        protected void DebugPrint(string s)
        {
            for (int i = 0; i < depth; i++)
            {
                Console.Write("   ");
            }
            Console.WriteLine(s);
        }

    }

}
