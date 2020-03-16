using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_assignment : AST_statement
    {
        private AST_identifier identifier;
        private AST_expression expression;

        public AST_identifier Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public AST_expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public AST_assignment(AST_identifier identifier, AST_expression expression)
        {
            this.identifier = identifier;
            this.expression = expression;
        }

        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}
