namespace Mini_PL
{
    class AST_program : ASTNode
    {
        private AST_statement_list statement_list;

        public AST_statement_list StatementList
        {
            get
            {
                return this.statement_list;
            }
        }

        public AST_program(AST_statement_list statement_list)
        {
            this.statement_list = statement_list;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
