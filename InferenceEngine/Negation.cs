using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class Negation : Clause
    {
        public Negation(Clause arg)
        {
            Arg = arg;
        }

        public Clause Arg { get; }

        public override string ToString()
        {
            return Arg is Symbol
                ? $"{Parser.Tokens[TokenType.NOT]}{Arg}"
                : $"{Parser.Tokens[TokenType.NOT]}({Arg})";
        }

        public override int GetHashCode()
        {
            return Arg.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj) && obj is Negation other)
            {
                return Arg.Equals(other.Arg);
            }
            return false;
        }

        public override Clause Negate()
        {
            return Arg;
        }

        public override bool? Evaluate(Dictionary<Symbol, bool> model)
        {
            bool? result = Arg.Evaluate(model);
            return result.HasValue ? !result.Value : (bool?)null;
        }

        public override HashSet<Symbol> Symbols()
        {
            return Arg.Symbols();
        }
    }
}
