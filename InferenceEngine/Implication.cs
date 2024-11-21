using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class Implication : Clause
    {
        public Implication(Clause antecedent, Clause consequent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public Clause Antecedent { get; }
        public Clause Consequent { get; }

        public override string ToString()
        {
            string antecedent = Antecedent is Symbol || Antecedent is Negation
                ? Antecedent.ToString()
                : $"({Antecedent})";

            string consequent = Consequent is Symbol || Consequent is Negation
                ? Consequent.ToString()
                : $"({Consequent})";

            return $"{antecedent} {Parser.Tokens[TokenType.IMPLIES]} {consequent}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Antecedent, Consequent);
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj) && obj is Implication other)
            {
                return Antecedent.Equals(other.Antecedent) && Consequent.Equals(other.Consequent);
            }
            return false;
        }

        public override Clause Negate()
        {
            return new Conjunction(Antecedent, Consequent.Negate());
        }

        public override bool? Evaluate(Dictionary<Symbol, bool> model)
        {
            bool? antecedentValue = Antecedent.Evaluate(model);
            bool? consequentValue = Consequent.Evaluate(model);

            if (antecedentValue == null || consequentValue == null)
            {
                return null;
            }

            return !antecedentValue.Value || consequentValue.Value;
        }

        public override HashSet<Symbol> Symbols()
        {
            var symbols = new HashSet<Symbol>(Antecedent.Symbols());
            symbols.UnionWith(Consequent.Symbols());
            return symbols;
        }
    }
}
