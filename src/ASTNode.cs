using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    abstract class ASTNode
    {
        public abstract void Accept(ASTVisitor visitor);
    }
}
