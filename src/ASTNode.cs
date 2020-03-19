using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    abstract class ASTNode
    {
        public abstract void Accept(ASTVisitor visitor);

        public int Row { get; set; }

        public int Column { get; set; }
    
    }
}
