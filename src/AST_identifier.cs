using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_identifier : AST_operand
    {
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public AST_identifier(string name)
        {
            this.name = name;
        }
        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}
