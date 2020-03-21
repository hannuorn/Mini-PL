namespace Mini_PL
{
    class AST_integer_literal : AST_operand
    {
        private string value;

        public string Value
        {
            get
            {
                return this.value;
            }
        }

        public AST_integer_literal(string value)
        {
            this.value = value;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
