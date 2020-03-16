using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_print_statement : AST_statement
    {
        private AST_expression expression;

        public AST_expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public AST_print_statement(AST_expression expression)
        {
            this.expression = expression;
        }

        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
