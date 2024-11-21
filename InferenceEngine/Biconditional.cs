using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class Biconditional : CommutativeClause
    {
        public Biconditional(params Clause[] arguments) : base(TokenType.BICONDITIONAL, ValidateArguments(arguments))
        {
        }

        public override Clause Negate()
        {
            var args = Arguments.ToArray();
            return new Biconditional(args[0], args[1].Negate());
        }

        public override bool? Evaluate(Dictionary<Symbol, bool> model)
        {
            var values = Arguments.Select(arg => arg.Evaluate(model)).ToArray();
            if (values.Contains(null))
                return null;
            return values[0] == values[1];
        }

        private static Clause[] ValidateArguments(Clause[] arguments)
        {
            if (arguments.Length != 2)
                throw new ArgumentException("Biconditional sentence must have exactly 2 arguments.");
            return arguments;
        }
    }
}
