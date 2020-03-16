using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_assert_statement : AST_statement
    {
        AST_expression expression;

        public AST_expression Expression
        {
            get
            {
                return this.expression;
            }
        }
        public AST_assert_statement(AST_expression expression)
        {
            this.expression = expression;
        }

        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
