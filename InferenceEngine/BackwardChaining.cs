using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class BackwardChaining
    {
        /// <summary>
        /// Initializes a new instance of the BackwardChaining class.
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base containing facts and rules.</param>
        /// <param name="query">The query to evaluate.</param>
        public BackwardChaining(KnowledgeBase knowledgeBase)
        {
            Base = knowledgeBase;
            InferenceChain = new List<Symbol>();

            if (!Base.Symbols.Contains(Base.Query))
            {
                throw new ArgumentException("The query is not a valid symbol in the knowledge base.");
            }
        }

        public KnowledgeBase Base { get; set; }
        public bool Entails { get; set; }
        public List<Symbol> InferenceChain { get; set; }

        /// <summary>
        /// Solves the query using backward chaining.
        /// </summary>
        /// <returns>Result indicating whether the query is entailed and the reasoning chain.</returns>
        public void Solve()
        {
            if (Base.Base.OfType<Symbol>().Any(fact => fact.Equals(Base.Query)))
            {
                // Query is directly a fact in the KB
                Entails = true;
                InferenceChain = new List<Symbol>(Base.Query.Symbols());
                return;
            }

            // Begin backward chaining
            var visited = new HashSet<Symbol>();

            Symbol startGoal = null;
            if (Base.Query is Symbol)   // Symbol and Implication (Symbol consequent) is allowed
            {
                startGoal = (Symbol)Base.Query;
            }
            else if (Base.Query is Implication)
            {
                var consequent = ((Implication)Base.Query).Consequent;
                if (consequent is Symbol)
                {
                    startGoal = (Symbol)consequent;
                }
                else
                {
                    Entails = false;    // Consequent must not be other clauses than single Symbol
                    return;
                }
            }
            else
            {
                Entails = false;    // If alone, query must be a single Symbol
                return;
            }

            Entails = Prove(startGoal, InferenceChain, visited);
        }

        /// <summary>
        /// Recursively attempts to prove the given goal.
        /// </summary>
        /// <param name="goal">The symbol to prove.</param>
        /// <param name="chain">The reasoning chain of proven symbols.</param>
        /// <param name="visited">The set of visited symbols to prevent cycles.</param>
        /// <returns>True if the goal can be proven; otherwise, false.</returns>
        private bool Prove(Symbol goal, List<Symbol> chain, HashSet<Symbol> visited)
        {
            if (visited.Contains(goal))
            {
                return false; // Prevent infinite loops due to cycles
            }

            visited.Add(goal);

            foreach (var clause in Base.Base)
            {
                // Check if the goal is directly a fact
                if (clause is Symbol fact && fact.Equals(goal))
                {
                    chain.Add(goal);
                    return true;
                }

                // Check if the goal is a consequent of an implication
                if (clause is Implication implication && implication.Consequent.Equals(goal))
                {
                    var antecedents = implication.Antecedent is Conjunction conjunction
                        ? conjunction.Arguments.ToList()
                        : new List<Clause> { implication.Antecedent };

                    // Attempt to prove all antecedents
                    bool allProven = true;
                    foreach (var antecedent in antecedents)
                    {
                        if (antecedent is Symbol subGoal && !chain.Contains(subGoal))
                        {
                            if (!Prove(subGoal, chain, visited))
                            {
                                allProven = false;
                                break;
                            }
                        }
                    }

                    if (allProven)
                    {
                        chain.Add(goal);
                        return true;
                    }
                }
            }

            return false; // Goal could not be proven
        }
    }
}
