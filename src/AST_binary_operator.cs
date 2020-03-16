using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_binary_operator : AST_expression
    {
        public enum OperatorKind
        {
            Plus,
            Minus,
            Asterisk,
            Slash,
            Less,
            Equal,
            Ampersand
        }

        private OperatorKind kind;
        private AST_operand left;
        private AST_operand right;

        public OperatorKind Kind
        {
            get
            { 
                return this.kind;
            }
        }

        public AST_operand LeftOperand
        {
            get
            {
                return this.left;
            }
        }
        public AST_operand RightOperand
        {
            get
            {
                return this.right;
            }
        }

        public AST_binary_operator(OperatorKind kind)
        {
            this.kind = kind;
        }

        public AST_binary_operator(OperatorKind kind, AST_operand left, AST_operand right)
        {
            this.kind = kind;
            this.left = left;
            this.right = right;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void SetLeft(AST_operand left)
        {
            this.left = left;
        }

        public void SetRight(AST_operand right)
        {
            this.right = right;
        }

    }
}
