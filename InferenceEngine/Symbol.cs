using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class Symbol : Clause
    {
        public Symbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj) && obj is Symbol other)
            {
                return Name == other.Name;
            }
            return false;
        }
        public override Clause Negate()
        {
            return new Negation(this);
        }

        public override bool? Evaluate(Dictionary<Symbol, bool> model)
        {
            return model.ContainsKey(this) ? model[this] : (bool?)null;
        }

        public override HashSet<Symbol> Symbols()
        {
            return new HashSet<Symbol> { this };
        }
    }
}
