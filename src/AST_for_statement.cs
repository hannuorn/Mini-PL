using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_for_statement : AST_statement
    {
        private AST_identifier identifier;
        private AST_expression from;
        private AST_expression to;
        private AST_statement_list statement_list;

        public AST_identifier Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public AST_expression From
        {
            get
            {
                return this.from;
            }
        }

        public AST_expression To
        {
            get
            {
                return this.to;
            }
        }

        public AST_statement_list StatementList
        {
            get
            {
                return this.statement_list;
            }
        }

        public AST_for_statement(AST_identifier identifier, AST_expression from,
            AST_expression to, AST_statement_list statement_list)
        {
            this.identifier = identifier;
            this.from = from;
            this.to = to;
            this.statement_list = statement_list;
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}
