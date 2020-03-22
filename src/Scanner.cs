using System;
using static Mini_PL.TokenKind;

namespace Mini_PL
{
    class Scanner
    {
        // Row and Column indicate the current position in the source code.
        private int Row = 0;
        private int Column = 0;
        private string[] Source;

        private bool errorsFound = false;

        // Remember last error position to suppress multiple errors at the same position.
        private int lastErrorRow = -1;
        private int lastErrorColumn = -1;

        public bool ErrorsFound
        {
            get
            {
                return this.errorsFound;
            }
        }

        public Scanner()
        {
        }

        // Print erroneous line of source code and the columm pointer.
        private void PrintSourceError(int row, int column)
        {
            if (row < Source.Length) {
                string s = Source[row];
                Console.WriteLine(s);
                for (int i = 0; i < column; i++)
                {
                    Console.Write("-");
                }
                Console.WriteLine("^\n");
            }
        }

        public void Error(string error, int row, int column)
        {
            if (row == lastErrorRow && column == lastErrorColumn) return;

            lastErrorRow = row;
            lastErrorColumn = column;

            string s = "[" + (row + 1) + ":" + (column + 1) + "]";
            int padding = 12 - s.Length;
            for (int i = 0; i < padding; i++)
            {
                s = string.Concat(s, " ");
            }
            s = string.Concat(s, error);
            Console.WriteLine(s);
            PrintSourceError(row, column);
            this.errorsFound = true;
        }

        public void Error(string error, Token token)
        {
            Error(error, token.Row, token.Column);
        }

        public void SetSource(string source)
        {
            Source = new string[] { source };
        }

        public void SetSource(string[] source)
        {
            Source = source;
        }

        public bool ReadSource(string fileName)
        {
            try
            {
                Source = System.IO.File.ReadAllLines(fileName);
                return true;
            } catch (Exception)
            {
                return false;
            }
        }

        public void Reset()
        {
            Row = 0;
            Column = 0;
        }

        public Token GetIntegerLiteral()
        {
            int row = Row;
            int column = Column;
            string numberString = "";
            numberString = CurrentCharacter().ToString();
            SkipCharacter();
            while (IsNumber(CurrentCharacter()))
            {
                numberString = numberString.Insert(numberString.Length, CurrentCharacter().ToString());
                SkipCharacter();
            }
            return new Token(TokenKind.int_Literal, numberString, row, column);
        }

        private Token GetIdentifierOrKeywordToken()
        {
            int row = Row;
            int column = Column;
            string id = CurrentCharacter().ToString();
            SkipCharacter();
            while (IsIdentifierCharacter(CurrentCharacter()))
            {
                id = id.Insert(id.Length, CurrentCharacter().ToString());
                SkipCharacter();
            }
            TokenKind kind = WordToTokenKind(id);
            return new Token(kind, id, row, column);
        }

        private bool IsEscapeSequence(string source, int index)
        {
            return
                PairOfCharactersEquals("\\\\", source, index) |
                PairOfCharactersEquals("\\\"", source, index) |
                PairOfCharactersEquals("\\n", source, index);
        }

        private string EscapeSequenceValue(string source, int index)
        {
            if (PairOfCharactersEquals("\\\\", source, index))
            {
                return "\\";
            } else if (PairOfCharactersEquals("\\\"", source, index)) {
                return "\"";
            } else if (PairOfCharactersEquals("\\n", source, index)) {
                return "\n";
            }
            return "";
        }

        private Token GetStringLiteralToken()
        {
            // Build the string one character at a time,
            // until terminating doublequote is found
            // or end of line is met.
            string s = "";

            int row = Row;
            int column = Column;

            // Skip the initial doublequote
            SkipCharacter();

            while (true)
            {
                if (Column >= Source[Row].Length)
                {
                    Error("String literal must terminate before end of line.", row, column);
                    break;
                }
                if (IsEscapeSequence(Source[Row], Column))
                {
                    s = s.Insert(s.Length, EscapeSequenceValue(Source[Row], Column));
                    SkipCharacters(2);
                } else
                {
                    if (Source[Row][Column].Equals('"'))
                    {
                        SkipCharacter();
                        break;
                    } else
                    {
                        s = s.Insert(s.Length, Source[Row][Column].ToString());
                        SkipCharacter();
                    }
                }
            }
            return new Token(string_Literal, s, row, column);
        }

        public Token GetNextToken()
        {
            SkipWhitespaceAndComments();

            if (AtEndOfSource())
            {
                return new Token(EndOfSource, "", Row, Column);
            }
            char c = CurrentCharacter();
            int row = Row;
            int column = Column;
            switch (c)
            {
                case ';':
                    SkipCharacter();
                    return new Token(Semicolon, ";", row, column);

                case '+':
                    SkipCharacter();
                    return new Token(Plus, "+", row, column);

                case '-':
                    SkipCharacter();
                    return new Token(Minus, "-", row, column);

                case '*':
                    SkipCharacter();
                    return new Token(Asterisk, "*", row, column);

                case '/':
                    SkipCharacter();
                    return new Token(Slash, "/", row, column);

                case '=':
                    SkipCharacter();
                    return new Token(Equal, "=", row, column);

                case '&':
                    SkipCharacter();
                    return new Token(Ampersand, "&", row, column);

                case '!':
                    SkipCharacter();
                    return new Token(Exclamation, "!", row, column);

                case '(':
                    SkipCharacter();
                    return new Token(OpenParenthesis, "(", row, column);

                case ')':
                    SkipCharacter();
                    return new Token(CloseParenthesis, ")", row, column);

                default:
                    break;
            }

            if (c == ':')
            {
                if (PairOfCharactersEquals(":="))
                {
                    SkipCharacters(2);
                    return new Token(Assignment, ":=", row, column);
                } else {
                    SkipCharacter();
                    return new Token(Colon, ":", row, column);
                }
            }

            if (c == '.')
            {
                if (PairOfCharactersEquals(".."))
                {
                    SkipCharacters(2);
                    return new Token(RangeDots, "..", row, column);
                } else
                {
                    Error("Single dot not allowed.", row, column);
                    SkipCharacter();
                    return new Token(ErrorToken, ".", row, column);
                }
            }

            if (IsNumber(c))
            {
                return GetIntegerLiteral();
            }

            if (IsIdentifierCharacter(c))
            {
                return GetIdentifierOrKeywordToken();
            }

            if (c == '"')
            {
                return GetStringLiteralToken();
            }
            SkipCharacter();
            Error("Invalid character: '" + c + "'.", row, column);
            return new Token(ErrorToken, c.ToString(), row, column);
        }

        private TokenKind WordToTokenKind(string word)
        {
            TokenKind kind;
            if      (word.Equals("false"))  kind = TokenKind.bool_Literal;
            else if (word.Equals("true"))   kind = TokenKind.bool_Literal;
            else if (word.Equals("var"))    kind = TokenKind.var_Keyword;
            else if (word.Equals("for"))    kind = TokenKind.for_Keyword;
            else if (word.Equals("end"))    kind = TokenKind.end_Keyword;
            else if (word.Equals("in"))     kind = TokenKind.in_Keyword;
            else if (word.Equals("do"))     kind = TokenKind.do_Keyword;
            else if (word.Equals("read"))   kind = TokenKind.read_Keyword;
            else if (word.Equals("print"))  kind = TokenKind.print_Keyword;
            else if (word.Equals("int"))    kind = TokenKind.int_Keyword;
            else if (word.Equals("string")) kind = TokenKind.string_Keyword;
            else if (word.Equals("bool"))   kind = TokenKind.bool_Keyword;
            else if (word.Equals("assert")) kind = TokenKind.assert_Keyword;
            else                            kind = TokenKind.Identifier;
            return kind;
        }

        private bool IsWhitespace(char c)
        {
            return (c == ' ' || c == '\t' || c == '\f' || c == '\n');
        }

        private bool IsNumber(char c)
        {
            return (c.CompareTo('0') >= 0 && c.CompareTo('9') <= 0);
        }

        private bool IsIdentifierCharacter(char c)
        {
            if      (c.CompareTo('A') >= 0 && c.CompareTo('Z') <= 0) return true;
            else if (c.CompareTo('a') >= 0 && c.CompareTo('z') <= 0) return true;
            else if (IsNumber(c)) return true;
            else if (c == '_') return true;
            else return false;
        }

        private void SkipCharacter()
        {
            if (Row < Source.Length)
            {
                if (Column < Source[Row].Length)
                {
                    Column++;
                }
                else
                {
                    Row++;
                    Column = 0;
                }
            }
        }

        private char CurrentCharacter()
        {
            char c = (char)0;
            if (!AtEndOfSource())
            {
                if (Column < Source[Row].Length)
                {
                    c = Source[Row][Column];
                } else
                {
                    c = '\n';
                }
            }
            return c;
        }

        private bool AtEndOfSource()
        {
            return (Row >= Source.Length);
        }

        private char GetCharacter()
        {
            if (AtEndOfSource())
            {
                return (char)0;
            } else
            {
                return Source[Row][Column];
            }
        }

        // Checks if the pair of characters at index matches a given pair.
        private bool PairOfCharactersEquals(string pair, string source, int index)
        {
            // Comparison possible only if at least 2 characters remaining on the line.
            if (source.Length - index >= 2)
            {
                return (source.Substring(index, 2).Equals(pair));
            } else
            {
                return false;
            }
        }

        // Checks if the pair of characters at current location matches a given pair.
        private bool PairOfCharactersEquals(string pair)
        {
            if (AtEndOfSource())
            {
                return false;
            } else
            {
                return PairOfCharactersEquals(pair, Source[Row], Column);
            }
        }

        private void SkipCharacters(int n)
        {
            for (int i = 0; i < n; i++)
            {
                SkipCharacter();
            }
        }

        private void SkipWhitespace()
        {
            while (!AtEndOfSource())
            {
                if (IsWhitespace(CurrentCharacter()))
                {
                    SkipCharacter();
                } else
                {
                    break;
                }
            }
        }

        private void SkipRow()
        {
            if (Row < Source.Length)
            {
                Row++;
                Column = 0;
            }
        }

        private void SkipUntilEndOfComment()
        {
            while (!AtEndOfSource())
            {
                // Recursion to handle nested comments
                if (PairOfCharactersEquals("/*"))
                {
                    SkipCharacters(2);
                    SkipUntilEndOfComment();
                } else if (PairOfCharactersEquals("*/"))
                {
                    SkipCharacters(2);
                    break;
                } else
                {
                    SkipCharacter();
                }
            }
        }

        private void SkipWhitespaceAndComments()
        {
            while (!AtEndOfSource())
            {
                int oldRow = Row;
                int oldColumn = Column;
                SkipWhitespace();
                if (PairOfCharactersEquals("//"))
                {
                    SkipRow();
                } else if (PairOfCharactersEquals("/*"))
                {
                    SkipCharacters(2);
                    SkipUntilEndOfComment();
                }
                if (Row == oldRow && Column == oldColumn)
                {
                    break;
                }
            }
        }
    }
}
