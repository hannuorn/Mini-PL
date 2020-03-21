using System;
using static Mini_PL.TokenKind;

namespace Mini_PL
{
	[Flags]
	public enum TokenKind
	{
		EndOfSource			= 0b_0000_0000_0000_0000_0000_0000_0000_0001,
		Colon				= 0b_0000_0000_0000_0000_0000_0000_0000_0010,
		Semicolon 			= 0b_0000_0000_0000_0000_0000_0000_0000_0100,
		OpenParenthesis 		= 0b_0000_0000_0000_0000_0000_0000_0000_1000,
		CloseParenthesis 	= 0b_0000_0000_0000_0000_0000_0000_0001_0000,
		Plus					= 0b_0000_0000_0000_0000_0000_0000_0010_0000,
		Minus				= 0b_0000_0000_0000_0000_0000_0000_0100_0000,
		Asterisk				= 0b_0000_0000_0000_0000_0000_0000_1000_0000,
		Slash				= 0b_0000_0000_0000_0000_0000_0001_0000_0000,
		Less					= 0b_0000_0000_0000_0000_0000_0010_0000_0000,
		Equal				= 0b_0000_0000_0000_0000_0000_0100_0000_0000,
		Ampersand			= 0b_0000_0000_0000_0000_0000_1000_0000_0000,
		Exclamation			= 0b_0000_0000_0000_0000_0001_0000_0000_0000,
		Assignment			= 0b_0000_0000_0000_0000_0010_0000_0000_0000,
		RangeDots			= 0b_0000_0000_0000_0000_0100_0000_0000_0000,
		int_Literal			= 0b_0000_0000_0000_0000_1000_0000_0000_0000,
		string_Literal		= 0b_0000_0000_0000_0001_0000_0000_0000_0000,
		bool_Literal			= 0b_0000_0000_0000_0010_0000_0000_0000_0000,
		var_Keyword			= 0b_0000_0000_0000_0100_0000_0000_0000_0000,
		for_Keyword			= 0b_0000_0000_0000_1000_0000_0000_0000_0000,
		end_Keyword			= 0b_0000_0000_0001_0000_0000_0000_0000_0000,
		in_Keyword			= 0b_0000_0000_0010_0000_0000_0000_0000_0000,
		do_Keyword			= 0b_0000_0000_0100_0000_0000_0000_0000_0000,
		read_Keyword			= 0b_0000_0000_1000_0000_0000_0000_0000_0000,
		print_Keyword		= 0b_0000_0001_0000_0000_0000_0000_0000_0000,
		int_Keyword			= 0b_0000_0010_0000_0000_0000_0000_0000_0000,
		string_Keyword		= 0b_0000_0100_0000_0000_0000_0000_0000_0000,
		bool_Keyword			= 0b_0000_1000_0000_0000_0000_0000_0000_0000,
		assert_Keyword		= 0b_0001_0000_0000_0000_0000_0000_0000_0000,
		Identifier			= 0b_0010_0000_0000_0000_0000_0000_0000_0000,
		ErrorToken			= 0b_0100_0000_0000_0000_0000_0000_0000_0000
	}

	public class Token
	{
		private TokenKind kind;
		private string lexeme;
		public int Row { get; set; }
		public int Column { get; set; }

		public Token(TokenKind kind, string lexeme, int row, int column)
		{
			this.kind = kind;
			this.lexeme = lexeme;
			this.Row = row;
			this.Column = column;
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

		public override string ToString()
		{
			switch (kind)
			{
				case EndOfSource:
					return "<end-of-source>";

				default:
					return lexeme;
			}
		}
	}
}
