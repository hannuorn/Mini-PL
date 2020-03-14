using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_statement_list : ASTNode
    {
        private List<AST_statement> statement_list;

        public AST_statement_list()
        {
            this.statement_list = new List<AST_statement>();
        }

        public void Add_statement(AST_statement statement)
        {
            this.statement_list.Add(statement);
        }
    }
}
