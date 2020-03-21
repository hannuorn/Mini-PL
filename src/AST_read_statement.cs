namespace Mini_PL
{
    class AST_read_statement : AST_statement
    {
        private AST_identifier identifier;

        public AST_identifier Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public AST_read_statement(AST_identifier identifier)
        {
            this.identifier = identifier;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
