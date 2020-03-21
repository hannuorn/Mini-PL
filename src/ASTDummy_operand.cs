namespace Mini_PL
{
    class ASTDummy_operand : AST_operand
    {
        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
