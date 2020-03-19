using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class EvaluateVisitor : ASTVisitor
    {
        private SortedDictionary<string, Variable> variables;
        private Variable value;

        public EvaluateVisitor()
        {
            this.variables = new SortedDictionary<string, Variable>();
            this.value = new Variable();
        }

        public Variable Value
        {
            get
            {
                return this.value;
            }
        }

        public void IncrementVariable(AST_identifier identifier)
        {
            Variable var = variables.GetValueOrDefault(identifier.Name);
            var.Set(var.IntValue + 1);
        }

        public void SetVariable(string name, int newValue)
        {
            Variable var = variables.GetValueOrDefault(name);
            var.Set(newValue);
        }

        public void SetVariable(string name, string newValue)
        {
            Variable var = variables.GetValueOrDefault(name);
            switch (var.Type)
            {
                case AST_type.AST_type_kind.bool_type:
                    if (newValue.Equals("false"))
                    {
                        var.Set(false);
                    } else if (newValue.Equals("true"))
                    {
                        var.Set(true);
                    }
                    break;

                case AST_type.AST_type_kind.int_type:
                    int i = int.Parse(newValue);
                    var.Set(i);
                    break;

                case AST_type.AST_type_kind.string_type:
                    var.Set(newValue);
                    break;

                default:
                    break;

            }
        }

        public override void Visit(AST_assignment assignment)
        {
            assignment.Expression.Accept(this);
            Variable var = variables.GetValueOrDefault(assignment.Identifier.Name);
            var.Set(value);
        }

        override public void Visit(AST_bool_literal bool_literal)
        {
            value.Set(bool_literal.Value);

        }

        override public void Visit(AST_integer_literal integer_literal)
        {
            value.Set(int.Parse(integer_literal.Value));
        }

        override public void Visit(AST_string_literal string_literal)
        {
            value.Set(string_literal.Value);
        }

        override public void Visit(AST_expression_operand expression_operand)
        {
            expression_operand.Expression.Accept(this);
        }

        override public void Visit(AST_identifier identifier)
        {
            this.value = variables.GetValueOrDefault(identifier.Name).Copy();
        }

        override public void Visit(AST_unary_operator unary_operator)
        {
            unary_operator.Operand.Accept(this);
            value.Set(!value.BoolValue);
        }

        private void EvaluateBooleanBinaryOperator(AST_binary_operator binary_operator, Variable left, Variable right)
        {
            switch (binary_operator.Kind)
            {
                case AST_binary_operator.OperatorKind.Ampersand:
                    this.value.Set(left.BoolValue & right.BoolValue);
                    break;

                case AST_binary_operator.OperatorKind.Equal:
                    this.value.Set(left.BoolValue == right.BoolValue);
                    break;

                default:
                    break;
            }
        }

        private void EvaluateIntegerBinaryOperator(AST_binary_operator binary_operator, Variable left, Variable right)
        {
            switch (binary_operator.Kind)
            {
                case AST_binary_operator.OperatorKind.Equal:
                    this.value.Set(left.IntValue == right.IntValue);
                    break;

                case AST_binary_operator.OperatorKind.Less:
                    this.value.Set(left.IntValue < right.IntValue);
                    break;

                case AST_binary_operator.OperatorKind.Plus:
                    this.value.Set(left.IntValue + right.IntValue);
                    break;

                case AST_binary_operator.OperatorKind.Minus:
                    this.value.Set(left.IntValue - right.IntValue);
                    break;

                case AST_binary_operator.OperatorKind.Asterisk:
                    this.value.Set(left.IntValue * right.IntValue);
                    break;

                case AST_binary_operator.OperatorKind.Slash:
                    this.value.Set(left.IntValue / right.IntValue);
                    break;

                default:
                    break;
            }
        }

        private void EvaluateStringBinaryOperator(AST_binary_operator binary_operator, Variable left, Variable right)
        {
            switch (binary_operator.Kind)
            {
                case AST_binary_operator.OperatorKind.Equal:
                    this.value.Set(left.StringValue.Equals(right.StringValue));
                    break;

                case AST_binary_operator.OperatorKind.Less:
                    this.value.Set(left.StringValue.CompareTo(right.StringValue) < 0);
                    break;

                case AST_binary_operator.OperatorKind.Plus:
                    this.value.Set(string.Concat(left.StringValue, right.StringValue));
                    break;

                default:
                    break;
            }

        }

        override public void Visit(AST_binary_operator binary_operator)
        {
            binary_operator.LeftOperand.Accept(this);
            Variable left = this.Value.Copy();
            binary_operator.RightOperand.Accept(this);
            Variable right = this.Value.Copy();
            switch (left.Type)
            {
                case AST_type.AST_type_kind.bool_type:
                    EvaluateBooleanBinaryOperator(binary_operator, left, right);
                    break;

                case AST_type.AST_type_kind.int_type:
                    EvaluateIntegerBinaryOperator(binary_operator, left, right);
                    break;

                case AST_type.AST_type_kind.string_type:
                    EvaluateStringBinaryOperator(binary_operator, left, right);
                    break;

                default:
                    break;

            }
        }

        override public void Visit(AST_variable_declaration variable_declaration)
        {
            Variable variable;
            if (variable_declaration.Expression != null)
            {
                variable_declaration.Expression.Accept(this);
                variable = value.Copy();
            } else {
                variable = new Variable(variable_declaration.Type.Kind);
            }
            string name = variable_declaration.Identifier.Name;
            if (variables.ContainsKey(name))
            {
                variables.Remove(name);
            }
            variables.Add(name, variable);
        }

    }
}
