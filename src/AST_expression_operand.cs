namespace Mini_PL
{
    class AST_expression_operand : AST_operand
    {
        private AST_expression expression;

        public AST_expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public AST_expression_operand(AST_expression expression)
        {
            this.expression = expression;
        }

        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
