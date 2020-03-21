namespace Mini_PL
{
    class AST_variable_declaration : AST_statement
    {
        private AST_identifier identifier;
        private AST_type type;
        private AST_expression expression;

        public AST_identifier Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public AST_type Type
        {
            get
            {
                return this.type;
            }
        }

        public AST_expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public AST_variable_declaration(AST_identifier identifier, AST_type type)
        {
            this.identifier = identifier;
            this.type = type;
        }

        public AST_variable_declaration(AST_identifier identifier, AST_type type, AST_expression expression)
        {
            this.identifier = identifier;
            this.type = type;
            this.expression = expression;
        }

        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
