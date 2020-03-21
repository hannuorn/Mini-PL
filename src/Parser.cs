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

        private const TokenKind FirstSet_statement =
            var_Keyword |
            Identifier |
            for_Keyword |
            read_Keyword |
            print_Keyword |
            assert_Keyword;

        private const TokenKind FirstSet_operand =
            int_Literal | 
            string_Literal | 
            bool_Literal | 
            Identifier | 
            OpenParenthesis;

        const TokenKind BinaryOperators =
            Plus | Minus | Asterisk | Slash | Less | Equal | Ampersand;

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
        // ReuseToken: 'unconsume' a token that was already consumed

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

        private void ReuseToken()
        {
            lastReadTokenConsumed = false;
        }

        private bool AtEndOfSource()
        {
            return (lastReadToken != null && lastReadToken.Kind == EndOfSource);
        }

        private bool Match(TokenKind kind)
        {
            Token token = GetNextToken();
            bool matches = (token.Kind == kind);
            if (!matches)
            {
                ReuseToken();
            }
            return matches;
        }

        private bool isMemberOf(TokenKind kind, TokenKind set)
        {
            return (kind & set) == kind;
        }

        private bool IsStatementStarter(TokenKind kind)
        {
            return isMemberOf(kind, FirstSet_statement);
        }

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
                    Error("';' expected, " + t.ToString() + " found.", t);
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
        }

        private AST_statement Parse_statement(TokenKind followSet)
        {
            IncrementDepth();
            DebugPrint("stmt");

            AST_statement statement;
            Token t = LookAheadToken();

            switch (t.Kind)
            {
                case var_Keyword:
                    statement = Parse_variable_declaration();
                    break;

                case Identifier:
                    statement = Parse_assignment(followSet);
                    break;

                case for_Keyword:
                    statement = Parse_for_statement();
                    break;

                case read_Keyword:
                    statement = Parse_read_statement();
                    break;

                case print_Keyword:
                    statement = Parse_print_statement();
                    break;

                case assert_Keyword:
                    statement = Parse_assert_statement(followSet);
                    break;

                default:
                    Error("Statement expected. '" + t.ToString() + "' does not start a statement.", t);
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
            AST_identifier name;
            AST_type type = null;
            AST_expression expression = null;

            Match(var_Keyword);
            name = Parse_identifier(Colon | Semicolon);
            if (!Match(Colon))
            {
                Error("':' expected.", lastReadToken);
                SkipUntilFollow(Semicolon);
            } else
            {
                type = Parse_type(Assignment | Semicolon);
                switch (LookAheadToken().Kind)
                {
                    case Assignment:
                        Match(Assignment);
                        expression = Parse_expression(Semicolon);
                        break;

                    case Semicolon:
                        break;

                    default:
                        Error("':=' or ';' expected.", LookAheadToken());
                        SkipUntilFollow(Semicolon);
                        break;
                }
            }
            variable_declaration = new AST_variable_declaration(name, type, expression);

            DecrementDepth();

            return variable_declaration;
        }

        private AST_assignment Parse_assignment(TokenKind followSet)
        {
            IncrementDepth();

            AST_assignment assignment;
            AST_identifier identifier;
            AST_expression expression = null;

            identifier = Parse_identifier(Assignment);
            if (!Match(Assignment))
            {
                Error("':=' expected.", lastReadToken);
                if (LookAheadToken().Kind != Semicolon)
                {
                    // Assume an incorrect assignment symbol and skip over it
                    GetNextToken();
                }
            }
            expression = Parse_expression(Semicolon);

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
                Error("Identifier expected, '" + t.ToString() + "' found.", t);
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

            AST_for_statement for_statement = null;
            AST_expression from = null;
            AST_expression to = null;
            AST_statement_list statement_list = null;

            Match(for_Keyword);
            AST_identifier identifier = Parse_identifier(in_Keyword);

            bool skipForDo = false;
            bool skipAll = false;
            if (!Match(in_Keyword))
            {
                Error("'in' expected.", lastReadToken);
                // Prefer to skip for .. do and still process the statement_list,
                // worst case is to skip everything until 'end for'
                SkipUntilFollow(do_Keyword | end_Keyword);
                if (LookAheadToken().Kind == end_Keyword)
                {
                    Match(end_Keyword);
                    Match(for_Keyword);
                    skipAll = true;
                } else
                {
                    Match(do_Keyword);
                    skipForDo = true;
                }
            }
            if (!skipAll)
            {
                if (!skipForDo)
                {
                    from = Parse_expression(RangeDots);
                    if (!Match(RangeDots))
                    {
                        SkipUntilFollow(do_Keyword | end_Keyword);
                        if (LookAheadToken().Kind == end_Keyword)
                        {
                            Match(end_Keyword);
                            Match(for_Keyword);
                            skipAll = true;
                        } else
                        {
                            Match(do_Keyword);
                            skipForDo = true;
                        }
                    }
                    if (!skipAll && !skipForDo)
                    {
                        to = Parse_expression(do_Keyword | end_Keyword);
                        if (!Match(do_Keyword))
                        {
                            Error("'do' expected.", lastReadToken);
                            ReuseToken();
                        }
                    }
                }
                if (!skipAll)
                {
                    statement_list = Parse_statement_list();
                    if (!Match(end_Keyword))
                    {
                        Error("'end for' or a statement expected.", lastReadToken);
                        SkipUntilFollow(Semicolon);
                    } else
                    {
                        if (!Match(for_Keyword))
                        {
                            Error("'end for' expected.", lastReadToken);
                            SkipUntilFollow(Semicolon);
                        }
                    }
                }
            }

            for_statement = new AST_for_statement(identifier, from, to, statement_list);

            DecrementDepth();

            return for_statement;
        }

        private AST_read_statement Parse_read_statement()
        {
            IncrementDepth();

            AST_read_statement read_statement;

            Match(read_Keyword);
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

            Match(print_Keyword);
            print_statement = new AST_print_statement(Parse_expression(Semicolon));

            DecrementDepth();

            return print_statement;
        }

        private AST_assert_statement Parse_assert_statement(TokenKind followSet)
        {
            IncrementDepth();
            DebugPrint("assert_statement");

            AST_assert_statement assert_statement = null;

            Match(assert_Keyword);
            if (LookAheadToken().Kind == OpenParenthesis)
            {
                Match(OpenParenthesis);
                assert_statement = new AST_assert_statement(
                    Parse_expression(followSet | CloseParenthesis));
                if (LookAheadToken().Kind == CloseParenthesis)
                {
                    Match(CloseParenthesis);
                } else
                {
                    Error("')' expected, '" + LookAheadToken().ToString() + "' found.", LookAheadToken());
                    SkipUntilFollow(CloseParenthesis | followSet);
                    if (LookAheadToken().Kind == CloseParenthesis)
                    {
                        Match(CloseParenthesis);
                    }
                }
            } else
            {
                Error("'(' expected, '" + LookAheadToken().ToString() + "' found.", LookAheadToken());
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
                        Error("Expression expected. '" + t.ToString() + "' does not start an expression.", t);
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
            Match(Exclamation);
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
                    Match(OpenParenthesis);
                    operand = new AST_expression_operand(Parse_expression(followSet | CloseParenthesis));
                    if (LookAheadToken().Kind == CloseParenthesis)
                    {
                        Match(CloseParenthesis);
                    } else
                    {
                        Error("')' expected, '" + LookAheadToken().ToString() + "' found.", LookAheadToken());
                        SkipUntilFollow(followSet | CloseParenthesis);
                        if (LookAheadToken().Kind == CloseParenthesis)
                        {
                            Match(CloseParenthesis);
                        }
                    }
                    break;

                default:
                    Error("Operand expected. '" + t.ToString() + "' does not start an operand.", t);
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
                case int_Keyword:
                    DebugPrint("type int");
                    type = new AST_type(int_type);
                    break;

                case string_Keyword:
                    DebugPrint("type string");
                    type = new AST_type(string_type);
                    break;

                case bool_Keyword:
                    DebugPrint("type bool");
                    type = new AST_type(bool_type);
                    break;

                default:
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

            Match(int_Literal);
            DebugPrint("int_Literal: " + lastReadToken.Lexeme);
            integer_literal = new AST_integer_literal(lastReadToken.Lexeme);

            DecrementDepth();

            return integer_literal;
        }

        private AST_string_literal Parse_string_literal()
        {
            IncrementDepth();

            AST_string_literal string_literal;

            Match(string_Literal);
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
                    Error("bool literal expected.", t);
                    SkipUntilFollow(followSet);
                }
            }
            else
            {
                Error("bool literal expected", t);
                SkipUntilFollow(followSet);
            }
            bool_literal = new AST_bool_literal(value);

            DecrementDepth();

            return bool_literal;
        }
    }
}
