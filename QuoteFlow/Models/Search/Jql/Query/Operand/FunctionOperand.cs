using System.Collections.Generic;
using System.Text;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    public class FunctionOperand : IOperand
    {
        // This is calculated on the fly. I don't care about memory affects. In the most likely case this will only be calculated
        // once, in the worst case this may be calculated multiple times (but all to the same value).
        private string _caseInsensitiveName;

        public FunctionOperand(string name)
            : this(name, new List<string>())
        {
        }

        public FunctionOperand(string name, params string[] args)
            : this(name, new List<string>(args))
        {
        }

        public FunctionOperand(string name, IList<string> args)
        {
            Name = name;
            Args = args;
        }

        /// <summary>
        /// The name that represents this Operand, null if the operand is unnamed.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<string> Args { get; set; }

        public string DisplayString
        {
            get
            {
                if (Args.Count == 0)
                {
                    return Name + "()";
                }
                var sb = new StringBuilder();
                sb.Append(Name).Append("(");
                bool first = true;
                foreach (string arg in Args)
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(arg);
                    first = false;
                }
                sb.Append(")");
                return sb.ToString();
            }
        }

        public T Accept<T>(IOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        private string CaseInsensitiveName
        {
            get { return _caseInsensitiveName ?? (_caseInsensitiveName = Name.ToLower()); }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = (FunctionOperand) obj;

            if (!Args.Equals(that.Args))
            {
                return false;
            }
            if (!CaseInsensitiveName.Equals(that.CaseInsensitiveName))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = CaseInsensitiveName.GetHashCode();
            result = 31 * result + Args.GetHashCode();
            return result;
        }
    }
}