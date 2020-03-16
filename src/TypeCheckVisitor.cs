using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class TypeCheckVisitor : ASTVisitor
    {
        override public void Visit(AST_assert_statement assert_statement)
        {
            base.Visit(assert_statement);
            // check: assert_statement.Expression.DataType == AST_type.AST_type_kind.bool_type
        }

        override public void Visit(AST_assignment assignment)
        {
            base.Visit(assignment.Expression);
            // check type of variable matches type of expression
        }

        override public void Visit(AST_binary_operator binary_operator)
        {
            base.Visit(binary_operator);
            // check type of left = type of right
            binary_operator.DataType = binary_operator.LeftOperand.DataType;
        }

        override public void Visit(AST_bool_literal bool_literal)
        {
            bool_literal.DataType = AST_type.AST_type_kind.bool_type;
        }

        override public void Visit(AST_expression_operand expression_operand)
        {
            base.Visit(expression_operand);
            expression_operand.DataType = expression_operand.Expression.DataType;
        }

        override public void Visit(AST_for_statement for_statement)
        {
            base.Visit(for_statement);
            // check variable is an integer
            // check from and to is integer types
        }

        override public void Visit(AST_identifier identifier)
        {
            base.Visit(identifier);
        }

        override public void Visit(AST_integer_literal integer_literal)
        {
            base.Visit(integer_literal);
            integer_literal.DataType = AST_type.AST_type_kind.int_type;
        }

        override public void Visit(AST_print_statement print_statement)
        {
            base.Visit(print_statement);
        }

        override public void Visit(AST_program program)
        {
            base.Visit(program);
        }

        override public void Visit(AST_read_statement read_statement)
        {
            base.Visit(read_statement);
        }

        override public void Visit(AST_statement_list statement_list)
        {
            base.Visit(statement_list);
        }

        override public void Visit(AST_string_literal string_literal)
        {
            base.Visit(string_literal);
            string_literal.DataType = AST_type.AST_type_kind.string_type;
        }

        override public void Visit(AST_type type)
        {
            base.Visit(type);
        }

        override public void Visit(AST_unary_operator unary_operator)
        {
            base.Visit(unary_operator);
            // check type of operand is bool
        }

        override public void Visit(AST_variable_declaration variable_declaration)
        {
            base.Visit(variable_declaration);
            // check type of expression matches declared type
            // add to symbol table
        }
    }
}
