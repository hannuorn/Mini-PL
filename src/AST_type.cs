using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_type : ASTNode
    {
        public enum AST_type_kind
        {
            int_type,
            string_type,
            bool_type
        }

        private AST_type_kind kind;

        public AST_type_kind Kind
        {
            get
            {
                return this.kind;
            }
        }

        public AST_type(AST_type_kind kind)
        {
            this.kind = kind;
        }

        public override void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
