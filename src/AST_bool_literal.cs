namespace Mini_PL
{
    class AST_bool_literal : AST_operand
    {
        private bool value;

        public bool Value
        {
            get
            {
                return this.value;
            }
        }

        public AST_bool_literal(bool value)
        {
            this.value = value;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
