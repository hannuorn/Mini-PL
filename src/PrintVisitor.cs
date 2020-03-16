using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class PrintVisitor : ASTVisitor
    {

        override public void Visit(AST_program program)
        {
            DebugPrint("program");
            base.Visit(program);
        }

        override public void Visit(AST_statement_list statement_list)
        {
            DebugPrint("statement_list: " + statement_list.statement_list.Count);
            base.Visit(statement_list);
        }

        override public void Visit(AST_assert_statement assert_statement)
        {
            DebugPrint("assert_statement");
            base.Visit(assert_statement);
        }

        override public void Visit(AST_assignment assignment)
        {
            DebugPrint("assignment");
            base.Visit(assignment);
        }

        public override void Visit(AST_binary_operator binary_operator)
        {
            DebugPrint("binary_operator: " + binary_operator.Kind.ToString() + ", type: " + binary_operator.DataType.ToString());
            base.Visit(binary_operator);
        }

        override public void Visit(AST_bool_literal bool_literal) 
        {
            DebugPrint("bool_literal: " + bool_literal.Value.ToString());
        }

        override public void Visit(AST_expression expression)
        {
            DebugPrint("expression");
            base.Visit(expression);
        }

        override public void Visit(AST_expression_operand expression_operand)
        {
            DebugPrint("expression_operand: " + expression_operand.DataType.ToString());
            base.Visit(expression_operand);
        }

        override public void Visit(AST_for_statement for_statement)
        {
            DebugPrint("for_statement");
            base.Visit(for_statement);
        }

        override public void Visit(AST_identifier identifier)
        {
            DebugPrint("identifier: " + identifier.Name);
        }

        override public void Visit(AST_integer_literal integer_literal)
        {
            DebugPrint("integer_literal: " + integer_literal.Value);
        }

        override public void Visit(AST_print_statement print_statement)
        {
            DebugPrint("print_statement");
            base.Visit(print_statement);
        }

        override public void Visit(AST_read_statement read_statement)
        {
            DebugPrint("read_statement");
            base.Visit(read_statement);
        }

        override public void Visit(AST_string_literal string_literal)
        {
            DebugPrint("string_literal: \"" + string_literal.Value + "\"");
        }

        override public void Visit(AST_type type)
        {
            DebugPrint("type: " + type.Kind.ToString());
        }
        override public void Visit(AST_unary_operator unary_operator)
        {
            DebugPrint("unary_operator");
            base.Visit(unary_operator);
        }

        override public void Visit(AST_variable_declaration variable_declaration)
        {
            DebugPrint("variable_declaration");
            base.Visit(variable_declaration);
        }

    }
}
