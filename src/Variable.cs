using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_PL
{
    class Variable
    {
        private string name;
        private AST_type.AST_type_kind type;
        private int intValue;
        private string stringValue;
        private bool boolValue;

        public Variable()
        {
            this.intValue = 0;
            this.stringValue = "";
            this.boolValue = false;
        }

        public Variable(AST_type.AST_type_kind type)
        {
            this.type = type;
            this.intValue = 0;
            this.stringValue = "";
            this.boolValue = false;
        }

        public Variable Copy()
        {
            Variable clone = new Variable(type);
            clone.name = name;
            clone.type = type;
            clone.intValue = intValue;
            clone.stringValue = stringValue;
            clone.boolValue = boolValue;
            return clone;
        }

        public void Set(Variable value)
        {
            type = value.Type;
            boolValue = value.BoolValue;
            intValue = value.IntValue;
            stringValue = value.StringValue;
        }

        public void Set(bool value)
        {
            type = AST_type.AST_type_kind.bool_type;
            boolValue = value;
        }

        public void Set(int value)
        {
            type = AST_type.AST_type_kind.int_type;
            intValue = value;
        }

        public void Set(string value)
        {
            type = AST_type.AST_type_kind.string_type;
            stringValue = value;
        }

        public AST_type.AST_type_kind Type
        {
            get
            {
                return this.type;
            }
        }

        public bool BoolValue
        {
            get
            {
                return this.boolValue;
            }
        }

        public int IntValue
        {
            get
            {
                return this.intValue;
            }
        }

        public string StringValue
        {
            get
            {
                return this.stringValue;
            }
        }

        public override string ToString()
        {
            switch (type)
            {
                case AST_type.AST_type_kind.bool_type:
                    return boolValue.ToString();

                case AST_type.AST_type_kind.int_type:
                    return intValue.ToString();

                case AST_type.AST_type_kind.string_type:
                    return stringValue;

                default:
                    return null;
            }
        }
    }
}
