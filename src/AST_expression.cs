namespace Mini_PL
{
    abstract class AST_expression : ASTNode
    {
        private AST_type.AST_type_kind dataType;

        public AST_type.AST_type_kind DataType
        {
            get
            {
                return this.dataType;
            }
            set
            {
                this.dataType = value;
            }
        }
    }
}
