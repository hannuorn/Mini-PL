using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_binary_operator : AST_expression
    {
        private AST_operand leftOperand;
        private AST_operand rightOperand;

        public void SetLeft(AST_operand left)
        {
            this.leftOperand = left;
        }

        public void SetRight(AST_operand right)
        {
            this.rightOperand = right;
        }

    }
}
