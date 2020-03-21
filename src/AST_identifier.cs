namespace Mini_PL
{
    class AST_identifier : AST_operand
    {
        public string Name { get; }
        public AST_variable_declaration Declaration { get; set; }

        public AST_identifier(string name)
        {
            Name = name;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
