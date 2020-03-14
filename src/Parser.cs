using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mini_PL
{

    class Parser
    {
        private Scanner scanner;

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
            Parse_program();
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

        private ASTNode Parse_program()
        {
            return Parse_statement_list();
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
                    statement = new AST_statement();
                    break;
            }

            DecrementDepth();

            return statement;
        }

        private AST_variable_declaration Parse_variable_declaration()
        {
            IncrementDepth();

            AST_variable_declaration variable_declaration = new AST_variable_declaration();

            Match(TokenKind.var_Keyword);
            Token varName = GetNextToken();
            Match(TokenKind.Colon);
            Token typeName = GetNextToken();
            DebugPrint("var_declaration: " + varName.Lexeme + " : " + typeName.Lexeme);
            if (LookAheadToken().Kind == TokenKind.Assignment)
            {
                Match(TokenKind.Assignment);
                Parse_expression();
            }

            DecrementDepth();

            return variable_declaration;
        }

        private AST_assignment Parse_assignment()
        {
            IncrementDepth();

            AST_assignment assignment = new AST_assignment();

            Token t = GetNextToken();
            DebugPrint("assignment" + t.Lexeme);
            // verify identifier
            Match(TokenKind.Assignment);
            Parse_expression();

            DecrementDepth();

            return assignment;
        }

        private AST_for_statement Parse_for_statement()
        {
            IncrementDepth();
            DebugPrint("for_statement");

            Match(TokenKind.for_Keyword);
            Token var_ident = GetNextToken();
            // verify ident
            Match(TokenKind.in_Keyword);
            AST_expression from = Parse_expression();
            Match(TokenKind.RangeDots);
            AST_expression to = Parse_expression();
            Match(TokenKind.do_Keyword);
            AST_statement_list statement_list = Parse_statement_list();
            Match(TokenKind.end_Keyword);
            Match(TokenKind.for_Keyword);

            AST_for_statement for_statement = new AST_for_statement(from, to, statement_list);

            DecrementDepth();

            return for_statement;
        }

        private AST_read_statement Parse_read_statement()
        {
            IncrementDepth();

            AST_read_statement read_statement = new AST_read_statement();

            Match(TokenKind.read_Keyword);
            Token var_ident = GetNextToken();
            DebugPrint("read_statement, var_ident = " + var_ident.Lexeme);

            DecrementDepth();

            return read_statement;
        }

        private AST_print_statement Parse_print_statement()
        {
            IncrementDepth();
            DebugPrint("print_statement");

            AST_print_statement print_statement = new AST_print_statement();

            Match(TokenKind.print_Keyword);
            Parse_expression();

            DecrementDepth();

            return print_statement;
        }

        private AST_assert_statement Parse_assert_statement()
        {
            IncrementDepth();
            DebugPrint("assert_statement");

            AST_assert_statement assert_statement = new AST_assert_statement();

            Match(TokenKind.assert_Keyword);
            Match(TokenKind.OpenParenthesis);
            Parse_expression();
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

            AST_unary_operator unary_operator = new AST_unary_operator();

            DebugPrint("unary_op_opnd");
            Match(TokenKind.Exclamation);
            unary_operator.SetOperand(Parse_operand());

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

            AST_binary_operator binary_operator = new AST_binary_operator();

            Token t = GetNextToken();
            DebugPrint("op " + t.Lexeme);
            switch (t.Kind)
            {
                case TokenKind.Plus:
                    break;

                default:
                    break;
            }

            DecrementDepth();

            return binary_operator;
        }

        private void Parse_type()
        {
            IncrementDepth();

            Token t = GetNextToken();
            switch (t.Kind)
            {
                case TokenKind.int_Keyword:
                    DebugPrint("type int");
                    break;

                case TokenKind.string_Keyword:
                    DebugPrint("type string");
                    break;

                case TokenKind.bool_Keyword:
                    DebugPrint("type bool");
                    break;

                default:
                    // Error
                    break;
            }

            DecrementDepth();
        }

        private AST_integer_literal Parse_integer_literal()
        {
            IncrementDepth();

            AST_integer_literal integer_literal = new AST_integer_literal();

            Match(TokenKind.int_Literal);
            DebugPrint("int_Literal: " + lastReadToken.Lexeme);

            DecrementDepth();

            return integer_literal;
        }

        private AST_string_literal Parse_string_literal()
        {
            IncrementDepth();

            AST_string_literal string_literal = new AST_string_literal();

            Match(TokenKind.string_Literal);
            DebugPrint("string_Literal: " + lastReadToken.Lexeme);

            DecrementDepth();

            return string_literal;
        }

        private AST_bool_literal Parse_bool_literal()
        {
            IncrementDepth();

            AST_bool_literal bool_literal = new AST_bool_literal();

            Match(TokenKind.bool_Literal);
            DebugPrint("bool_Literal: " + lastReadToken.Lexeme);

            DecrementDepth();

            return bool_literal;
        }

        private AST_identifier Parse_identifier()
        {
            IncrementDepth();
            DebugPrint("var_ident");

            AST_identifier identifier = new AST_identifier();

            Match(TokenKind.Identifier);
            DebugPrint("ident: " + lastReadToken.Lexeme);

            DecrementDepth();

            return identifier;
        }

    }

}
