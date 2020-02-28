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
		int_Literal,
		string_Literal,
		bool_Literal,
		var_Keyword,
		for_Keyword,
		end_Keyword,
		in_Keyword,
		do_Keyword,
		read_Keyword,
		print_Keyword,
		int_Keyword,
		string_Keyword,
		bool_Keyword,
		assert_Keyword,
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
