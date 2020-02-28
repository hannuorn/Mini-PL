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
            Parse_prog();
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

        private void Parse_prog()
        {
            Parse_stmts();
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

        private void Parse_stmts()
        {
            IncrementDepth();
            DebugPrint("stmts");
            do
            {
                Parse_stmt();
                Match(TokenKind.Semicolon);
            } while (IsStatementStarter(LookAheadToken().Kind));
            DecrementDepth();
        }

        private void Parse_stmt()
        {
            IncrementDepth();
            DebugPrint("stmt");
            switch (LookAheadToken().Kind)
            {
                case TokenKind.var_Keyword:
                    Parse_var_declaration();
                    break;

                case TokenKind.Identifier:
                    Parse_assignment();
                    break;

                case TokenKind.for_Keyword:
                    Parse_for_statement();
                    break;

                case TokenKind.read_Keyword:
                    Parse_read_statement();
                    break;

                case TokenKind.print_Keyword:
                    Parse_print_statement();
                    break;

                case TokenKind.assert_Keyword:
                    Parse_assert_statement();
                    break;

                default:
                    // Error
                    break;
            }
            DecrementDepth();
        }

        private void Parse_var_declaration()
        {
            IncrementDepth();
            Match(TokenKind.var_Keyword);
            Token varName = GetNextToken();
            Match(TokenKind.Colon);
            Token typeName = GetNextToken();
            DebugPrint("var_declaration: " + varName.Lexeme + " : " + typeName.Lexeme);
            if (LookAheadToken().Kind == TokenKind.Assignment)
            {
                Match(TokenKind.Assignment);
                Parse_expr();
            }
            DecrementDepth();
        }

        private void Parse_assignment()
        {
            IncrementDepth();
            Token t = GetNextToken();
            DebugPrint("assignment" + t.Lexeme);
            // verify identifier
            Match(TokenKind.Assignment);
            Parse_expr();
            DecrementDepth();
        }

        private void Parse_for_statement()
        {
            IncrementDepth();
            DebugPrint("for_statement");
            Match(TokenKind.for_Keyword);
            Token var_ident = GetNextToken();
            // verify ident
            Match(TokenKind.in_Keyword);
            Parse_expr();
            Match(TokenKind.RangeDots);
            Parse_expr();
            Match(TokenKind.do_Keyword);
            Parse_stmts();
            Match(TokenKind.end_Keyword);
            Match(TokenKind.for_Keyword);
            DecrementDepth();
        }

        private void Parse_read_statement()
        {
            IncrementDepth();
            Match(TokenKind.read_Keyword);
            Token var_ident = GetNextToken();
            DebugPrint("read_statement, var_ident = " + var_ident.Lexeme);
            DecrementDepth();
        }

        private void Parse_print_statement()
        {
            IncrementDepth();
            DebugPrint("print_statement");
            Match(TokenKind.print_Keyword);
            Parse_expr();
            DecrementDepth();
        }

        private void Parse_assert_statement()
        {
            IncrementDepth();
            DebugPrint("assert_statement");
            Match(TokenKind.assert_Keyword);
            Match(TokenKind.OpenParenthesis);
            Parse_expr();
            Match(TokenKind.CloseParenthesis);
            DecrementDepth();
        }

        private void Parse_expr()
        {
            IncrementDepth();
            DebugPrint("expr");
            switch (LookAheadToken().Kind)
            {
                case TokenKind.Exclamation:
                    Parse_unary_op_opnd();
                    break;

                default:
                    Parse_opnd();
                    // Mini-PL does not allow 1 + 2 + 3,
                    // only (1 + 2) + 3  or  1 + (2 + 3).
                    // Change this 'if' to 'while' to remove
                    // this restriction.
                    if (IsBinaryOperator(LookAheadToken().Kind))
                    {
                        Parse_op();
                        Parse_opnd();
                    }
                    break;
            }
            DecrementDepth();
        }

        private void Parse_unary_op_opnd()
        {
            IncrementDepth();
            DebugPrint("unary_op_opnd");
            Match(TokenKind.Exclamation);
            Parse_opnd();
            DecrementDepth();
        }

        private void Parse_opnd()
        {
            IncrementDepth();
            DebugPrint("opnd");
            switch (LookAheadToken().Kind)
            {
                case TokenKind.int_Literal:
                    Parse_int_Literal();
                    break;

                case TokenKind.string_Literal:
                    Parse_string_Literal();
                    break;

                case TokenKind.bool_Literal:
                    Parse_bool_Literal();
                    break;

                case TokenKind.Identifier:
                    Parse_var_ident();
                    break;

                default:
                    Match(TokenKind.OpenParenthesis);
                    Parse_expr();
                    Match(TokenKind.CloseParenthesis);
                    break;
            }
            DecrementDepth();
        }

        private void Parse_op()
        {
            IncrementDepth();
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

        private void Parse_int_Literal()
        {
            IncrementDepth();
            Match(TokenKind.int_Literal);
            DebugPrint("int_Literal: " + lastReadToken.Lexeme);
            DecrementDepth();
        }

        private void Parse_string_Literal()
        {
            IncrementDepth();
            Match(TokenKind.string_Literal);
            DebugPrint("string_Literal: " + lastReadToken.Lexeme);
            DecrementDepth();
        }

        private void Parse_bool_Literal()
        {
            IncrementDepth();
            Match(TokenKind.bool_Literal);
            DebugPrint("bool_Literal: " + lastReadToken.Lexeme);
            DecrementDepth();
        }

        private void Parse_var_ident()
        {
            IncrementDepth();
            DebugPrint("var_ident");
            Parse_ident();
            DecrementDepth();
        }

        private void Parse_ident()
        {
            IncrementDepth();
            Match(TokenKind.Identifier);
            DebugPrint("ident: " + lastReadToken.Lexeme);
            DecrementDepth();
        }

    }

}
