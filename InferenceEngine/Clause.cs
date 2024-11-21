using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public abstract class Clause
    {
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public abstract override string ToString();

        public abstract override int GetHashCode();

        public abstract Clause Negate();

        public abstract bool? Evaluate(Dictionary<Symbol, bool> model);

        public abstract HashSet<Symbol> Symbols();
    }
}
