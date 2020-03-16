using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_statement_list : ASTNode
    {
        private List<AST_statement> _statement_list;

        public List<AST_statement> statement_list
        {
            get
            {
                return _statement_list;
            }
        }

        public AST_statement_list()
        {
            this._statement_list = new List<AST_statement>();
        }

        override public void Accept(ASTVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Add_statement(AST_statement statement)
        {
            this.statement_list.Add(statement);
        }
    }
}
