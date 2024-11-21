using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public abstract class CommutativeClause : Clause
    {
        public CommutativeClause(TokenType connective, params Clause[] arguments)
        {
            if (arguments.Length < 2)
                throw new ArgumentException($"Commutative sentence {connective.ToString()} must have at least two arguments.");

            if (connective == TokenType.IMPLIES || connective == TokenType.NOT)
                throw new ArgumentException($"Connective {connective.ToString()} is not commutative.");

            Connective = connective;
            Arguments = new HashSet<Clause>(arguments);
        }

        public TokenType Connective { get; }
        public HashSet<Clause> Arguments { get; }

        public override string ToString()
        {
            var sortedArgs = Arguments.OrderBy(arg => arg.ToString()).ToList();
            return string.Join($" {Parser.Tokens[Connective]} ", sortedArgs.Select(arg =>
                arg is Symbol || arg is Negation ? arg.ToString() : $"({arg})"));
        }

        public override int GetHashCode()
        {
            return Arguments.Aggregate(0, (hash, arg) => hash ^ arg.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj is CommutativeClause other &&
                   Connective == other.Connective &&
                   Arguments.SetEquals(other.Arguments);
        }

        public override HashSet<Symbol> Symbols()
        {
            return Arguments.SelectMany(arg => arg.Symbols()).ToHashSet();
        }

        public abstract override bool? Evaluate(Dictionary<Symbol, bool> model);
    }
}
