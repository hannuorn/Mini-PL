using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_unary_operator : AST_expression
    {
        private AST_operand operand;

        public AST_operand Operand
        {
            get
            {
                return this.operand;
            }
        }

        public AST_unary_operator(AST_operand operand)
        {
            this.operand = operand;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
