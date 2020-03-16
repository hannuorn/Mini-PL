using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mini_PL
{

    class Parser
    {
        private Scanner scanner;
        private AST_program program;

        // This is the 'one token buffer' required for LL (1) parsing.
        private Token lastReadToken = null;
        private bool lastReadTokenConsumed = false;

        // Debugging aids
        private int depth = 0;


        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public void Parse()
        {
            this.program = Parse_program();
        }

        public AST_program Get_program()
        {
            return this.program;
        }

        // GetNextToken: read and consume
        // LookAheadToken: read but do not consume

        private Token GetNextToken()
        {
            if (lastReadToken != null && !lastReadTokenConsumed)
            {
                lastReadTokenConsumed = true;
                return lastReadToken;
            } else
            {
                lastReadToken = scanner.GetNextToken();
                lastReadTokenConsumed = true;
                return lastReadToken;
            }
        }

        private Token LookAheadToken()
        {
            if (lastReadToken != null && !lastReadTokenConsumed)
            {
                return lastReadToken;
            } else
            {
                lastReadToken = scanner.GetNextToken();
                lastReadTokenConsumed = false;
                return lastReadToken;
            }
        }

        private bool AtEndOfSource()
        {
            return (lastReadToken != null && lastReadToken.Kind == TokenKind.EndOfSource);
        }

        private void Match(TokenKind kind)
        {
            Token token = GetNextToken();
            // verify token kind
            Debug.Assert(token.Kind == kind);
        }

        public bool IsBinaryOperator(TokenKind kind)
        {
            return (
                kind == TokenKind.Plus ||
                kind == TokenKind.Minus ||
                kind == TokenKind.Asterisk ||
                kind == TokenKind.Slash ||
                kind == TokenKind.Less ||
                kind == TokenKind.Equal ||
                kind == TokenKind.Ampersand);
        }

        private void IncrementDepth()
        {
            depth++;
        }

        private void DecrementDepth()
        {
            depth--;
        }

        private void DebugPrint(string s)
        {
            for (int i = 0; i < depth; i++)
            {
                Console.Write("  ");
            }
            Console.WriteLine(s);
        }

        private AST_program Parse_program()
        {
            return new AST_program(Parse_statement_list());
        }

        private bool IsStatementStarter(TokenKind kind)
        {
            return (
                kind == TokenKind.var_Keyword ||
                kind == TokenKind.Identifier ||
                kind == TokenKind.for_Keyword ||
                kind == TokenKind.read_Keyword ||
                kind == TokenKind.print_Keyword ||
                kind == TokenKind.assert_Keyword);
        }

        private AST_statement_list Parse_statement_list()
        {
            IncrementDepth();
            DebugPrint("statement_list");

            AST_statement_list statement_list = new AST_statement_list();

            do
            {
                statement_list.Add_statement(Parse_statement());
                Match(TokenKind.Semicolon);
            } while (IsStatementStarter(LookAheadToken().Kind));

            DecrementDepth();
            return statement_list;
        }

        private AST_statement Parse_statement()
        {
            IncrementDepth();
            DebugPrint("stmt");

            AST_statement statement;

            switch (LookAheadToken().Kind)
            {
                case TokenKind.var_Keyword:
                    statement = Parse_variable_declaration();
                    break;

                case TokenKind.Identifier:
                    statement = Parse_assignment();
                    break;

                case TokenKind.for_Keyword:
                    statement = Parse_for_statement();
                    break;

                case TokenKind.read_Keyword:
                    statement = Parse_read_statement();
                    break;

                case TokenKind.print_Keyword:
                    statement = Parse_print_statement();
                    break;

                case TokenKind.assert_Keyword:
                    statement = Parse_assert_statement();
                    break;

                default:
                    // Error
                    statement = null;
                    break;
            }

            DecrementDepth();

            return statement;
        }

        private AST_variable_declaration Parse_variable_declaration()
        {
            IncrementDepth();

            AST_variable_declaration variable_declaration;

            Match(TokenKind.var_Keyword);
            AST_identifier name = Parse_identifier();
            Match(TokenKind.Colon);
            AST_type type = Parse_type();
            DebugPrint("variable_declaration");
            if (LookAheadToken().Kind == TokenKind.Assignment)
            {
                Match(TokenKind.Assignment);
                AST_expression expression = Parse_expression();
                variable_declaration = new AST_variable_declaration(name, type, expression);
            } else
            {
                variable_declaration = new AST_variable_declaration(name, type);
            }

            DecrementDepth();

            return variable_declaration;
        }

        private AST_assignment Parse_assignment()
        {
            IncrementDepth();

            AST_assignment assignment;

            AST_identifier identifier = Parse_identifier();
            Match(TokenKind.Assignment);
            AST_expression expression = Parse_expression();

            assignment = new AST_assignment(identifier, expression);
            DecrementDepth();

            return assignment;
        }

        private AST_identifier Parse_identifier()
        {
            IncrementDepth();

            AST_identifier identifier;

            Token t = GetNextToken();
            identifier = new AST_identifier(t.Lexeme);
            DebugPrint("identifier: " + t.Lexeme);

            DecrementDepth();

            return identifier;
        }

        private AST_for_statement Parse_for_statement()
        {
            IncrementDepth();
            DebugPrint("for_statement");

            Match(TokenKind.for_Keyword);
            AST_identifier identifier = Parse_identifier();
            Match(TokenKind.in_Keyword);
            AST_expression from = Parse_expression();
            Match(TokenKind.RangeDots);
            AST_expression to = Parse_expression();
            Match(TokenKind.do_Keyword);
            AST_statement_list statement_list = Parse_statement_list();
            Match(TokenKind.end_Keyword);
            Match(TokenKind.for_Keyword);

            AST_for_statement for_statement = new AST_for_statement(identifier, from, to, statement_list);

            DecrementDepth();

            return for_statement;
        }

        private AST_read_statement Parse_read_statement()
        {
            IncrementDepth();

            AST_read_statement read_statement;

            Match(TokenKind.read_Keyword);
            AST_identifier identifier = Parse_identifier();

            DebugPrint("read_statement, identifier = " + identifier.Name);

            read_statement = new AST_read_statement(identifier);

            DecrementDepth();

            return read_statement;
        }

        private AST_print_statement Parse_print_statement()
        {
            IncrementDepth();
            DebugPrint("print_statement");

            AST_print_statement print_statement;

            Match(TokenKind.print_Keyword);
            print_statement = new AST_print_statement(Parse_expression());

            DecrementDepth();

            return print_statement;
        }

        private AST_assert_statement Parse_assert_statement()
        {
            IncrementDepth();
            DebugPrint("assert_statement");

            AST_assert_statement assert_statement;

            Match(TokenKind.assert_Keyword);
            Match(TokenKind.OpenParenthesis);
            assert_statement = new AST_assert_statement(Parse_expression());
            Match(TokenKind.CloseParenthesis);

            DecrementDepth();

            return assert_statement;
        }

        private AST_expression Parse_expression()
        {
            IncrementDepth();
            DebugPrint("expr");

            AST_expression expression;

            switch (LookAheadToken().Kind)
            {
                case TokenKind.Exclamation:
                    expression = Parse_unary_operator_operand();
                    break;

                default:
                    AST_operand left_operand = Parse_operand();
                    if (IsBinaryOperator(LookAheadToken().Kind))
                    {
                        AST_binary_operator binary_operator = Parse_binary_operator();
                        AST_operand right_operand = Parse_operand();
                        binary_operator.SetLeft(left_operand);
                        binary_operator.SetRight(right_operand);
                        expression = binary_operator;
                    } else
                    {
                        expression = left_operand;
                    }

                    break;
            }

            DecrementDepth();

            return expression;
        }

        private AST_unary_operator Parse_unary_operator_operand()
        {
            IncrementDepth();

            AST_unary_operator unary_operator;

            DebugPrint("unary_op_opnd");
            Match(TokenKind.Exclamation);
            unary_operator = new AST_unary_operator(Parse_operand());

            DecrementDepth();

            return unary_operator;
        }

        private AST_operand Parse_operand()
        {
            IncrementDepth();
            DebugPrint("opnd");

            AST_operand operand;

            switch (LookAheadToken().Kind)
            {
                case TokenKind.int_Literal:
                    operand = Parse_integer_literal();
                    break;

                case TokenKind.string_Literal:
                    operand = Parse_string_literal();
                    break;

                case TokenKind.bool_Literal:
                    operand = Parse_bool_literal();
                    break;

                case TokenKind.Identifier:
                    operand = Parse_identifier();
                    break;

                default:
                    Match(TokenKind.OpenParenthesis);
                    operand = new AST_expression_operand(Parse_expression());
                    Match(TokenKind.CloseParenthesis);
                    break;
            }

            DecrementDepth();

            return operand;
        }

        private AST_binary_operator Parse_binary_operator()
        {
            IncrementDepth();

            AST_binary_operator binary_operator;

            Token t = GetNextToken();
            DebugPrint("op " + t.Lexeme);
            AST_binary_operator.OperatorKind kind;
            switch (t.Kind)
            {
                case TokenKind.Plus:
                    kind = AST_binary_operator.OperatorKind.Plus;
                    break;

                case TokenKind.Minus:
                    kind = AST_binary_operator.OperatorKind.Minus;
                    break;

                case TokenKind.Asterisk:
                    kind = AST_binary_operator.OperatorKind.Asterisk;
                    break;

                case TokenKind.Slash:
                    kind = AST_binary_operator.OperatorKind.Slash;
                    break;

                case TokenKind.Less:
                    kind = AST_binary_operator.OperatorKind.Less;
                    break;

                case TokenKind.Equal:
                    kind = AST_binary_operator.OperatorKind.Equal;
                    break;

                case TokenKind.Ampersand:
                    kind = AST_binary_operator.OperatorKind.Ampersand;
                    break;

                default:
                    kind = AST_binary_operator.OperatorKind.Plus;
                    break;
            }

            binary_operator = new AST_binary_operator(kind);

            DecrementDepth();

            return binary_operator;
        }

        private AST_type Parse_type()
        {
            IncrementDepth();

            AST_type type;

            Token t = GetNextToken();
            switch (t.Kind)
            {
                case TokenKind.int_Keyword:
                    DebugPrint("type int");
                    type = new AST_type(AST_type.AST_type_kind.int_type);
                    break;

                case TokenKind.string_Keyword:
                    DebugPrint("type string");
                    type = new AST_type(AST_type.AST_type_kind.string_type);
                    break;

                case TokenKind.bool_Keyword:
                    DebugPrint("type bool");
                    type = new AST_type(AST_type.AST_type_kind.bool_type);
                    break;

                default:
                    // Error
                    type = new AST_type(AST_type.AST_type_kind.bool_type);
                    break;
            }

            DecrementDepth();

            return type;
        }

        private AST_integer_literal Parse_integer_literal()
        {
            IncrementDepth();

            AST_integer_literal integer_literal;

            Match(TokenKind.int_Literal);
            DebugPrint("int_Literal: " + lastReadToken.Lexeme);
            integer_literal = new AST_integer_literal(lastReadToken.Lexeme);

            DecrementDepth();

            return integer_literal;
        }

        private AST_string_literal Parse_string_literal()
        {
            IncrementDepth();

            AST_string_literal string_literal;

            Match(TokenKind.string_Literal);
            DebugPrint("string_Literal: " + lastReadToken.Lexeme);

            string_literal = new AST_string_literal(lastReadToken.Lexeme);

            DecrementDepth();

            return string_literal;
        }

        private AST_bool_literal Parse_bool_literal()
        {
            IncrementDepth();

            AST_bool_literal bool_literal;

            Match(TokenKind.bool_Literal);
            DebugPrint("bool_Literal: " + lastReadToken.Lexeme);
            bool value = false;
            if (lastReadToken.Lexeme.Equals("false"))
            {
                value = false;
            }
            else if (lastReadToken.Lexeme.Equals("true"))
            {
                value = true;
            }
            else
            {
                // Error
            }
            bool_literal = new AST_bool_literal(value);

            DecrementDepth();

            return bool_literal;
        }

    }

}
