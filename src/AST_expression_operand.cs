using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_expression_operand : AST_operand
    {
        private AST_expression expression;

        public AST_expression_operand(AST_expression expression)
        {
            this.expression = expression;
        }
    }
}
