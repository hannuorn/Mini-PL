using System;
using System.Collections.Generic;
using System.Text;
using static Mini_PL.AST_type.AST_type_kind;
using static Mini_PL.AST_binary_operator.OperatorKind;


namespace Mini_PL
{
    class TypeCheckVisitor : ASTVisitor
    {
        private Scanner scanner;
        private SortedDictionary<string, AST_variable_declaration> variables;
        private SortedDictionary<string, bool> loopVariables;
        private bool errorsFound = false;

        public bool ErrorsFound
        {
            get { return this.errorsFound; }
        }

        public TypeCheckVisitor(Scanner scanner)
        {
            this.scanner = scanner;
            this.variables = new SortedDictionary<string, AST_variable_declaration>();
            this.loopVariables = new SortedDictionary<string, bool>();
        }

        private void Error(string error, ASTNode node)
        {
            scanner.Error(error, node.Row, node.Column);
            this.errorsFound = true;
        }

        override public void Visit(AST_assert_statement assert_statement)
        {
            if (assert_statement != null)
            {
                base.Visit(assert_statement);

                if (assert_statement.Expression.DataType != bool_type)
                {
                    Error("Assert statement requires a bool type.", assert_statement.Expression);
                }
            }
        }

        override public void Visit(AST_assignment assignment)
        {
            base.Visit(assignment);
            string name = assignment.Identifier.Name;
            if (variables.ContainsKey(name))
            {
                if (assignment.Expression.DataType == variables.GetValueOrDefault(name).Type.Kind)
                {
                    if (loopVariables.ContainsKey(name))
                    {
                        Error("'" + name + "' is a loop variable, assignment not allowed.", assignment.Identifier);
                    }
                } else
                {
                    Error("Type mismatch in assignment.", assignment);
                }
            } else
            {
                Error("Variable '" + name + "' has not been declared.", assignment);
            }
        }

        override public void Visit(AST_binary_operator binary_operator)
        {
            base.Visit(binary_operator);
            // check type of left = type of right
            if (binary_operator.LeftOperand.DataType != binary_operator.RightOperand.DataType)
            {
                Error("Left and right operands of binary operator must be of same type.", binary_operator);
            } else
            {
                AST_type.AST_type_kind t = binary_operator.LeftOperand.DataType;
                switch (binary_operator.Kind)
                {
                    case Ampersand:
                        if (t != bool_type)
                        {
                            Error("Binary operator '&' requires bool operands.", binary_operator);
                        }
                        break;

                    case Asterisk:
                        if (t != int_type)
                        {
                            Error("Binary operator '*' requires int operands.", binary_operator);
                        }
                        break;

                    case Equal:
                        break;

                    case Less:
                        break;

                    case Minus:
                        if (t != int_type)
                        {
                            Error("Binary operator '-' requires int operands.", binary_operator);
                        }
                        break;

                    case Plus:
                        if (t == bool_type)
                        {
                            Error("Binary operator '+' requires int or string operands.", binary_operator);
                        }
                        break;

                    case Slash:
                        if (t != int_type)
                        {
                            Error("Binary operator '/' requires int operands.", binary_operator);
                        }
                        break;

                    default:
                        break;
                }
            }
            
            switch (binary_operator.Kind)
            {
                case Equal:
                case Less:
                case Ampersand:
                    binary_operator.DataType = bool_type;
                    break;

                default:
                    binary_operator.DataType = binary_operator.LeftOperand.DataType;
                    break;
            }
        }

        override public void Visit(AST_bool_literal bool_literal)
        {
            bool_literal.DataType = bool_type;
        }

        override public void Visit(AST_expression_operand expression_operand)
        {
            base.Visit(expression_operand);
            if (expression_operand.Expression != null) {
                expression_operand.DataType = expression_operand.Expression.DataType;
            }
        }

        override public void Visit(AST_for_statement for_statement)
        {
            for_statement.Identifier.Accept(this);
            for_statement.From.Accept(this);
            for_statement.To.Accept(this);

            string name = for_statement.Identifier.Name;
            bool variableReserved = false;
            AST_variable_declaration declaration = variables.GetValueOrDefault(name);
            if (declaration != null)
            {
                if (variables.GetValueOrDefault(name).Type.Kind == int_type)
                {
                    if (loopVariables.ContainsKey(for_statement.Identifier.Name))
                    {
                        Error("'" + name + "' is already a loop variable.", for_statement.Identifier);
                    } else
                    {
                        loopVariables.Add(name, false);
                        variableReserved = true;
                    }
                }
                else
                {
                    Error("Loop variable must be of int type.", for_statement.Identifier);
                }
            }
            if (for_statement.From.DataType != int_type)
            {
                // from must be int
                Error("Lower boundary must be of int type.", for_statement.From);
            }
            if (for_statement.To.DataType != int_type)
            {
                Error("Upper boundary must be of int type.", for_statement.To);
            }
            for_statement.StatementList.Accept(this);
            if (variableReserved)
            {
                loopVariables.Remove(name);
            }
        }

        override public void Visit(AST_identifier identifier)
        {
            base.Visit(identifier);
            string name = identifier.Name;
            if (name.Length > 0)
            {
                if (variables.ContainsKey(name))
                {
                    identifier.Declaration = variables.GetValueOrDefault(name);
                    identifier.DataType = identifier.Declaration.Type.Kind;
                }
                else
                {
                    Error("Variable '" + name + "' has not been declared.", identifier);
                }
            }
        }

        override public void Visit(AST_integer_literal integer_literal)
        {
            base.Visit(integer_literal);
            integer_literal.DataType = int_type;
        }

        override public void Visit(AST_print_statement print_statement)
        {
            base.Visit(print_statement);
            if (print_statement.Expression.DataType == bool_type)
            {
                Error("Print statement requires int or string type.", print_statement.Expression);
            }
        }

        override public void Visit(AST_program program)
        {
            base.Visit(program);
        }

        override public void Visit(AST_read_statement read_statement)
        {
            base.Visit(read_statement);
            if (read_statement.Identifier.Declaration.Type.Kind == bool_type)
            {
                Error("Read statement requires int or string type.", read_statement.Identifier);
            }
        }

        override public void Visit(AST_statement_list statement_list)
        {
            base.Visit(statement_list);
        }

        override public void Visit(AST_string_literal string_literal)
        {
            base.Visit(string_literal);
            string_literal.DataType = string_type;
        }

        override public void Visit(AST_type type)
        {
            base.Visit(type);
        }

        override public void Visit(AST_unary_operator unary_operator)
        {
            base.Visit(unary_operator);
            if (unary_operator.Operand.DataType != bool_type)
            {
                // Einarinari
                Error("Unary operator '!' requires a bool operand.", unary_operator);
            }
        }

        override public void Visit(AST_variable_declaration variable_declaration)
        {
            variable_declaration.Type.Accept(this);
            string name = variable_declaration.Identifier.Name;
            if (variables.ContainsKey(name))
            {
                Error("Variable '" + name + "' has already been declared.", variable_declaration.Identifier);
            } else
            {
                AST_expression expr = variable_declaration.Expression;
                if (expr != null)
                {
                    expr.Accept(this);
                    if (expr.DataType != variable_declaration.Type.Kind)
                    {
                        Error("Variable and initial value must be same type.", variable_declaration.Expression);
                    }
                }
                variables.Add(name, variable_declaration);
            }
            variable_declaration.Identifier.Accept(this);
        }
    }
}
