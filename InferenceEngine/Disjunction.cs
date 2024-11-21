using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class Disjunction : CommutativeClause
    {
        public Disjunction(params Clause[] arguments)
            : base(TokenType.OR, FlattenArguments(arguments).ToArray())
        {
        }

        public override Clause Negate()
        {
            return new Conjunction(Arguments.Select(arg => arg.Negate()).ToArray());
        }

        public override bool? Evaluate(Dictionary<Symbol, bool> model)
        {
            var values = Arguments.Select(arg => arg.Evaluate(model)).ToList();
            if (values.Contains(null))
                return null;
            return values.Any(value => value == true);
        }

        private static List<Clause> FlattenArguments(Clause[] arguments)
        {
            var flattened = new List<Clause>();
            foreach (var arg in arguments)
            {
                if (arg is Disjunction disjunction)
                {
                    flattened.AddRange(disjunction.Arguments);
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
