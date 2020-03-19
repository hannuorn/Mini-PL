using System;
using System.Collections.Generic;
using System.Text;


namespace Mini_PL
{
    class ASTDummy_statement : AST_statement
    {
        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
