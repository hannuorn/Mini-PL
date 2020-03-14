using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_unary_operator : AST_expression
    {
        private AST_operand operand;

        public void SetOperand(AST_operand operand)
        {
            this.operand = operand;
        }
    }
}
