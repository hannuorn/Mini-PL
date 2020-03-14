using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class AST_for_statement : AST_statement
    {
        private AST_expression from;
        private AST_expression to;
        private AST_statement_list statement_list;

        public AST_for_statement(AST_expression from, AST_expression to, AST_statement_list statement_list)
        {
            this.from = from;
            this.to = to;
            this.statement_list = statement_list;
        }
    }
}
