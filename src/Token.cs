using System;
using System.Collections;


namespace Mini_PL
{
	public enum TokenKind
	{
		EndOfSource,
		Colon,
		Semicolon,
		OpenParenthesis,
		CloseParenthesis,
		Plus,
		Minus,
		Asterisk,
		Slash,
		Less,
		Equal,
		Ampersand,
		Exclamation,
		Assignment,
		RangeDots,
		Literal_int,
		Literal_string,
		Literal_bool_false,
		Literal_bool_true,
		Keyword_var,
		Keyword_for,
		Keyword_end,
		Keyword_in,
		Keyword_do,
		Keyword_read,
		Keyword_print,
		Keyword_int,
		Keyword_string,
		Keyword_bool,
		Keyword_assert,
		Identifier
	}

	public class Token
	{
		private TokenKind kind;
		private string lexeme;
		private int row;
		private int column;

		public Token(TokenKind kind, string lexeme, int row, int column)
		{
			this.kind = kind;
			this.lexeme = lexeme;
			this.row = row;
			this.column = column;
		}

		public Token(TokenKind kind)
		{
			this.kind = kind;
		}

		public TokenKind Kind
		{
			get
			{
				return kind;
			}
		}

		public string Lexeme
		{
			get
			{
				return lexeme;
			}
		}
	}
}
