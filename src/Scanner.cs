using System;
using System.Collections.Generic;
using System.Text;


namespace Mini_PL
{
    class Scanner
    {
        private int Row = 0;
        private int Column = 0;
        private string[] Source;

        public Scanner()
        {
        }

        public void SetSource(string source)
        {
            Source = new string[] { source };
        }

        public void SetSource(string[] source)
        {
            Source = source;
        }

        public void ReadSource(string fileName)
        {
            Source = System.IO.File.ReadAllLines(fileName);
        }

        public void Reset()
        {
            Row = 0;
            Column = 0;
        }

        public Token GetNextToken()
        {
            SkipWhitespaceAndComments();
            int row = Row;
            int column = Column;

            if (AtEndOfSource())
            {
                return new Token(TokenKind.EndOfSource, "", row, column);
            }
            char c = CurrentCharacter();
            switch (c)
            {
                case ';':
                    SkipCharacter();
                    return new Token(TokenKind.Semicolon, ";", row, column);

                case '+':
                    SkipCharacter();
                    return new Token(TokenKind.Plus, "+", row, column);

                case '-':
                    SkipCharacter();
                    return new Token(TokenKind.Minus, "-", row, column);

                case '*':
                    SkipCharacter();
                    return new Token(TokenKind.Asterisk, "*", row, column);

                case '/':
                    SkipCharacter();
                    return new Token(TokenKind.Slash, "/", row, column);

                case '=':
                    SkipCharacter();
                    return new Token(TokenKind.Equal, "=", row, column);

                case '&':
                    SkipCharacter();
                    return new Token(TokenKind.Ampersand, "&", row, column);

                case '!':
                    SkipCharacter();
                    return new Token(TokenKind.Exclamation, "!", row, column);

                case '(':
                    SkipCharacter();
                    return new Token(TokenKind.OpenParenthesis, "(", row, column);

                case ')':
                    SkipCharacter();
                    return new Token(TokenKind.CloseParenthesis, ")", row, column);
            }

            if (c == ':')
            {
                if (PairOfCharactersEquals(":="))
                {
                    SkipCharacters(2);
                    return new Token(TokenKind.Assignment, ":=", row, column);
                } else {
                    SkipCharacter();
                    return new Token(TokenKind.Colon, ":", row, column);
                }
            }

            if (c == '.')
            {
                if (PairOfCharactersEquals(".."))
                {
                    SkipCharacters(2);
                    return new Token(TokenKind.RangeDots, "..", row, column);
                } else
                {
                    // This is an error, Mini-PL has no single dot.
                    // For now, we'll pretend it is a double dot.
                    SkipCharacter();
                    return new Token(TokenKind.RangeDots, ".", row, column);
                }
            }

            if (IsNumber(c))
            {
                string numberString = "";
                numberString = c.ToString();
                SkipCharacter();
                while (IsNumber(CurrentCharacter()))
                {
                    numberString = numberString.Insert(numberString.Length, CurrentCharacter().ToString());
                    SkipCharacter();
                }
                return new Token(TokenKind.int_Literal, numberString, row, column);
            }

            if (IsIdentifierCharacter(c))
            {
                string id = c.ToString();
                SkipCharacter();
                while (IsIdentifierCharacter(CurrentCharacter()))
                {
                    id = id.Insert(id.Length, CurrentCharacter().ToString());
                    SkipCharacter();
                }
                TokenKind kind = WordToTokenKind(id);
                return new Token(kind, id, row, column);
            }

            if (c == '"')
            {
                int endOfString = Source[Row].IndexOf('"', Column + 1);
                if (endOfString >= 0)
                {
                    int length = endOfString - Column - 1;
                    string stringLiteral = Source[Row].Substring(Column + 1, length);
                    SkipCharacters(length + 2);
                    return new Token(TokenKind.string_Literal, stringLiteral, row, column);
                } else
                {
                    // Error: string literal does not terminate before end-of-line.
                    // For now, let's add the missing termination and accept the literal.
                    int length = Source[Row].Length - Column + 1;
                    string stringLiteral = Source[Row].Substring(Column, length - 1) + "\"";
                    SkipCharacters(length - 1);
                    return new Token(TokenKind.string_Literal, stringLiteral, row, column);
                }
            }
            return null;
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

        // Checks if the pair of characters at current location matches a given pair.
        private bool PairOfCharactersEquals(string pair)
        {
            if (AtEndOfSource())
            {
                return false;
            } else
            {
                // Comparison possible only if at least 2 characters remaining on the line.
                if (Source[Row].Length - Column >= 2)
                {
                    return (Source[Row].Substring(Column, 2).Equals(pair));
                } else
                {
                    return false;
                }
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
                if (PairOfCharactersEquals("*)"))
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
                } else if (PairOfCharactersEquals("(*"))
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
