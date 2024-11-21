using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class Conjunction : CommutativeClause
    {
        public Conjunction(params Clause[] arguments)
            : base(TokenType.AND, FlattenArguments(arguments).ToArray())
        {
        }

        public override Clause Negate()
        {
            return new Disjunction(Arguments.Select(arg => arg.Negate()).ToArray());
        }

        public override bool? Evaluate(Dictionary<Symbol, bool> model)
        {
            var values = Arguments.Select(arg => arg.Evaluate(model)).ToList();
            if (values.Contains(null))
                return null;
            return values.All(value => value == true);
        }

        private static List<Clause> FlattenArguments(Clause[] arguments)
        {
            var flattened = new List<Clause>();
            foreach (var arg in arguments)
            {
                if (arg is Conjunction conjunction)
                {
                    flattened.AddRange(conjunction.Arguments);
                }
                else
                {
                    flattened.Add(arg);
                }
            }
            return flattened;
        }
    }
}
