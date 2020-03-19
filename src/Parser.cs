using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Mini_PL.AST_type.AST_type_kind;
using static Mini_PL.TokenKind;


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

        public bool ErrorsFound { get; set; }

        const TokenKind FirstSet_operand =
            int_Literal | string_Literal | bool_Literal | Identifier | OpenParenthesis;


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

        private bool Match(TokenKind kind)
        {
            Token token = GetNextToken();
            bool matches = (token.Kind == kind);
            if (!matches)
            {
                // Error(kind.ToString() + " expected, " + token.Kind.ToString() + " found.");
            }
            return matches;
        }

        const TokenKind BinaryOperators =
            Plus | Minus | Asterisk | Slash | Less | Equal | Ampersand;

        public bool IsBinaryOperator(TokenKind kind)
        {
            return isMemberOf(kind, BinaryOperators);
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
            /*
            for (int i = 0; i < depth; i++)
            {
                Console.Write("  ");
            }
            Console.WriteLine(s);
            */
        }

        private void SkipUntilFollow(TokenKind followSet)
        {
            while (!isMemberOf(LookAheadToken().Kind, followSet | EndOfSource))
            {
                GetNextToken();
            }
        }

        private AST_program Parse_program()
        {
            AST_statement_list statement_list = Parse_statement_list();
            Match(EndOfSource);
            return new AST_program(statement_list);
        }

        private bool isMemberOf(TokenKind kind, TokenKind set)
        {
            return (kind & set) == kind;
        }

        private const TokenKind FirstSet_statement =
            TokenKind.var_Keyword |
            TokenKind.Identifier |
            TokenKind.for_Keyword |
            TokenKind.read_Keyword |
            TokenKind.print_Keyword |
            TokenKind.assert_Keyword;

        private bool IsStatementStarter(TokenKind kind)
        {
            return isMemberOf(kind, FirstSet_statement);
        }

        private AST_statement_list Parse_statement_list()
        {
            IncrementDepth();
            DebugPrint("statement_list");

            Token t = LookAheadToken();
            AST_statement_list statement_list = new AST_statement_list();

            do
            {
                statement_list.Add_statement(Parse_statement(Semicolon));
                t = LookAheadToken();
                if (t.Kind == Semicolon)
                {
                    Match(Semicolon);
                } else
                {
                    Error("';' expected, '" + t.Lexeme + "' found.", t);
                    SkipUntilFollow(Semicolon);
                    if (!AtEndOfSource())
                    {
                        Match(Semicolon);
                    }
                }
            } while (IsStatementStarter(LookAheadToken().Kind));

            statement_list.Row = t.Row;
            statement_list.Column = t.Column;

            DecrementDepth();
            return statement_list;
        }

        private void Error(string error, Token token)
        {
            scanner.Error(error, token);
            ErrorsFound = true;
        }

        private AST_statement Parse_statement(TokenKind followSet)
        {
            IncrementDepth();
            DebugPrint("stmt");

            AST_statement statement;
            Token t = LookAheadToken();

            switch (t.Kind)
            {
                case TokenKind.var_Keyword:
                    statement = Parse_variable_declaration();
                    break;

                case TokenKind.Identifier:
                    statement = Parse_assignment(followSet);
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
                    statement = Parse_assert_statement(followSet);
                    break;

                default:
                    Error("Statement expected. '" + t.Lexeme + "' does not start a statement.", t);
                    SkipUntilFollow(followSet);
                    statement = new ASTDummy_statement();
                    break;
            }
            if (statement != null)
            {
                statement.Row = t.Row;
                statement.Column = t.Column;
            }

            DecrementDepth();

            return statement;
        }

        private AST_variable_declaration Parse_variable_declaration()
        {
            IncrementDepth();

            AST_variable_declaration variable_declaration;

            Match(TokenKind.var_Keyword);
            AST_identifier name = Parse_identifier(Colon);
            Match(TokenKind.Colon);
            AST_type type = Parse_type(Assignment | Semicolon);
            DebugPrint("variable_declaration");
            if (LookAheadToken().Kind == TokenKind.Assignment)
            {
                Match(TokenKind.Assignment);
                AST_expression expression = Parse_expression(Semicolon);
                variable_declaration = new AST_variable_declaration(name, type, expression);
            } else
            {
                variable_declaration = new AST_variable_declaration(name, type);
            }

            DecrementDepth();

            return variable_declaration;
        }

        private AST_assignment Parse_assignment(TokenKind followSet)
        {
            IncrementDepth();

            AST_assignment assignment;

            AST_identifier identifier = Parse_identifier(Assignment);
            Match(TokenKind.Assignment);
            AST_expression expression = Parse_expression(Semicolon);

            assignment = new AST_assignment(identifier, expression);
            DecrementDepth();

            return assignment;
        }

        private AST_identifier Parse_identifier(TokenKind followSet)
        {
            IncrementDepth();

            AST_identifier identifier;

            Token t = GetNextToken();
            if (t.Kind == Identifier)
            {
                identifier = new AST_identifier(t.Lexeme);
                identifier.Row = t.Row;
                identifier.Column = t.Column;
                DebugPrint("identifier: " + t.Lexeme);
            } else
            {
                Error("Identifier expected, '" + t.Lexeme + "' found.", t);
                SkipUntilFollow(followSet);
                identifier = new AST_identifier("");
            }

            DecrementDepth();

            return identifier;
        }

        private AST_for_statement Parse_for_statement()
        {
            IncrementDepth();
            DebugPrint("for_statement");

            Match(TokenKind.for_Keyword);
            AST_identifier identifier = Parse_identifier(in_Keyword);
            Match(TokenKind.in_Keyword);
            AST_expression from = Parse_expression(RangeDots);
            Match(TokenKind.RangeDots);
            AST_expression to = Parse_expression(do_Keyword);
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
            AST_identifier identifier = Parse_identifier(Semicolon);

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
            print_statement = new AST_print_statement(Parse_expression(Semicolon));

            DecrementDepth();

            return print_statement;
        }

        private AST_assert_statement Parse_assert_statement(TokenKind followSet)
        {
            IncrementDepth();
            DebugPrint("assert_statement");

            AST_assert_statement assert_statement = null;

            Match(TokenKind.assert_Keyword);
            if (LookAheadToken().Kind == OpenParenthesis)
            {
                Match(TokenKind.OpenParenthesis);
                assert_statement = new AST_assert_statement(
                    Parse_expression(followSet | CloseParenthesis));
                if (LookAheadToken().Kind == CloseParenthesis)
                {
                    Match(TokenKind.CloseParenthesis);
                } else
                {
                    Error("')' expected, '" + LookAheadToken().Lexeme + "' found.", LookAheadToken());
                    SkipUntilFollow(followSet);
                }
            } else
            {
                Error("'(' expeced, '" + LookAheadToken().Lexeme + "' found.", LookAheadToken());
                SkipUntilFollow(followSet);
            }

            DecrementDepth();

            return assert_statement;
        }

        private AST_expression Parse_expression(TokenKind followSet)
        {
            IncrementDepth();
            DebugPrint("expr");

            AST_expression expression;

            Token t = LookAheadToken();
            switch (t.Kind)
            {
                case Exclamation:
                    expression = Parse_unary_operator_operand(followSet);
                    break;

                default:
                    if (isMemberOf(t.Kind, FirstSet_operand)) {
                        AST_operand left_operand = Parse_operand(followSet | BinaryOperators);
                        if (IsBinaryOperator(LookAheadToken().Kind))
                        {
                            AST_binary_operator binary_operator = Parse_binary_operator(followSet);
                            AST_operand right_operand = Parse_operand(followSet);
                            binary_operator.SetLeft(left_operand);
                            binary_operator.SetRight(right_operand);
                            expression = binary_operator;
                        } else
                        {
                            expression = left_operand;
                        }
                    } else
                    {
                        Error("Expression expected. '" + t.Lexeme + "' does not start an expression.", t);
                        expression = new ASTDummy_operand();
                        SkipUntilFollow(followSet);
                    }

                    break;
            }

            DecrementDepth();

            return expression;
        }

        private AST_unary_operator Parse_unary_operator_operand(TokenKind followSet)
        {
            IncrementDepth();

            Token t = LookAheadToken();
            AST_unary_operator unary_operator;

            DebugPrint("unary_op_opnd");
            Match(TokenKind.Exclamation);
            unary_operator = new AST_unary_operator(Parse_operand(followSet));
            unary_operator.Row = t.Row;
            unary_operator.Column = t.Column;

            DecrementDepth();

            return unary_operator;
        }

        private AST_operand Parse_operand(TokenKind followSet)
        {
            IncrementDepth();
            DebugPrint("opnd");

            Token t = LookAheadToken();
            AST_operand operand;

            switch (t.Kind)
            {
                case int_Literal:
                    operand = Parse_integer_literal();
                    break;

                case string_Literal:
                    operand = Parse_string_literal();
                    break;

                case bool_Literal:
                    operand = Parse_bool_literal(followSet);
                    break;

                case Identifier:
                    operand = Parse_identifier(followSet);
                    break;

                case OpenParenthesis:
                    Match(TokenKind.OpenParenthesis);
                    operand = new AST_expression_operand(Parse_expression(followSet | CloseParenthesis));
                    if (LookAheadToken().Kind == CloseParenthesis)
                    {
                        Match(CloseParenthesis);
                    } else
                    {
                        Error("')' expected, '" + LookAheadToken().Lexeme + "' found.", LookAheadToken());
                        SkipUntilFollow(followSet);
                    }
                    break;

                default:
                    Error("Operand expected. '" + t.Lexeme + "' does not start an operand.", t);
                    SkipUntilFollow(followSet);
                    operand = new ASTDummy_operand();
                    break;
            }

            operand.Row = t.Row;
            operand.Column = t.Column;

            DecrementDepth();

            return operand;
        }

        private AST_binary_operator Parse_binary_operator(TokenKind followSet)
        {
            IncrementDepth();

            AST_binary_operator binary_operator;

            Token t = GetNextToken();
            DebugPrint("op " + t.Lexeme);
            AST_binary_operator.OperatorKind kind;
            switch (t.Kind)
            {
                case Plus:
                    kind = AST_binary_operator.OperatorKind.Plus;
                    break;

                case Minus:
                    kind = AST_binary_operator.OperatorKind.Minus;
                    break;

                case Asterisk:
                    kind = AST_binary_operator.OperatorKind.Asterisk;
                    break;

                case Slash:
                    kind = AST_binary_operator.OperatorKind.Slash;
                    break;

                case Less:
                    kind = AST_binary_operator.OperatorKind.Less;
                    break;

                case Equal:
                    kind = AST_binary_operator.OperatorKind.Equal;
                    break;

                case Ampersand:
                    kind = AST_binary_operator.OperatorKind.Ampersand;
                    break;

                default:
                    Error("Binary operator expected.", t);
                    kind = AST_binary_operator.OperatorKind.Plus;
                    SkipUntilFollow(followSet);
                    break;
            }

            binary_operator = new AST_binary_operator(kind);
            binary_operator.Row = t.Row;
            binary_operator.Column = t.Column;

            DecrementDepth();

            return binary_operator;
        }

        private AST_type Parse_type(TokenKind followSet)
        {
            IncrementDepth();

            AST_type type;

            Token t = GetNextToken();
            switch (t.Kind)
            {
                case TokenKind.int_Keyword:
                    DebugPrint("type int");
                    type = new AST_type(int_type);
                    break;

                case TokenKind.string_Keyword:
                    DebugPrint("type string");
                    type = new AST_type(string_type);
                    break;

                case TokenKind.bool_Keyword:
                    DebugPrint("type bool");
                    type = new AST_type(bool_type);
                    break;

                default:
                    // Error
                    Error("Type expected.", t);
                    SkipUntilFollow(followSet);
                    type = new AST_type(bool_type);
                    break;
            }

            DecrementDepth();

            type.Row = t.Row;
            type.Column = t.Column;

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

        private AST_bool_literal Parse_bool_literal(TokenKind followSet)
        {
            IncrementDepth();

            AST_bool_literal bool_literal;

            Token t = GetNextToken();
            DebugPrint("bool_Literal: " + t.Lexeme);
            bool value = false;
            if (t.Kind == bool_Literal)
            {
                if (t.Lexeme.Equals("false"))
                {
                    value = false;
                }
                else if (t.Lexeme.Equals("true"))
                {
                    value = true;
                } else
                {
                    Error("bool literal expected, '" + t.Lexeme + "' found.", t);
                    SkipUntilFollow(followSet);
                }
            }
            else
            {
                Error("bool literal expected, " + t.Kind.ToString() + " found.", t);
                SkipUntilFollow(followSet);
            }
            bool_literal = new AST_bool_literal(value);

            DecrementDepth();

            return bool_literal;
        }

    }

}
