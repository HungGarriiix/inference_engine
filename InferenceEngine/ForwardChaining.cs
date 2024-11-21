using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class ForwardChaining
    {
        /// <summary>
        /// Initializes a new instance of the ForwardChaining solver.
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base containing clauses and the query.</param>
        public ForwardChaining(KnowledgeBase knowledgeBase)
        {
            Base = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            ValidateHornQuery();
        }

        public KnowledgeBase Base { get; set; }
        public bool Entails { get; set; }
        public List<Symbol> InferenceChain { get; set; }

        /// <summary>
        /// Solves the query using Forward Chaining.
        /// </summary>
        /// <returns>A ForwardChainingResult object containing whether the query is entailed and the inference chain.</returns>
        public void Solve()
        {
            // Initialize inferred and count dictionaries
            var inferred = Base.Base
                .SelectMany(clause => clause.Symbols())
                .Distinct()
                .ToDictionary(symbol => symbol, _ => false);

            var count = new Dictionary<Implication, int>();

            // Initialize the agenda with symbols known to be true
            var agenda = InitializeAgenda(Base.Base, count);

            // Track the result of forward chaining
            var chain = new List<Symbol>();

            while (agenda.Count > 0)
            {
                // Dequeue the first symbol
                var p = agenda.Dequeue();
                chain.Add(p);

                // If the query is found, return the result
                if (p.Equals(Base.Query))
                {
                    Entails = true;
                    InferenceChain = chain;
                    return;
                }

                // Mark symbol as inferred and propagate to implications
                if (!inferred[p])
                {
                    inferred[p] = true;

                    foreach (var clause in Base.Base.OfType<Implication>())
                    {
                        if (clause.Antecedent.Symbols().Contains(p))
                        {
                            count[clause]--;

                            if (count[clause] == 0)
                            {
                                foreach (Symbol symbol in clause.Consequent.Symbols())
                                    agenda.Enqueue(symbol);
                            }
                        }
                    }
                }
            }

            Entails = false;
            InferenceChain = chain;
        }

        /// <summary>
        /// Initializes the agenda and counts for implications.
        /// </summary>
        private Queue<Symbol> InitializeAgenda(List<Clause> baseClauses, Dictionary<Implication, int> count)
        {
            var agenda = new Queue<Symbol>();

            foreach (var clause in baseClauses)
            {
                if (clause is Symbol symbol)
                {
                    agenda.Enqueue(symbol);
                }
                else if (clause is Implication implication)
                {
                    count[implication] = implication.Antecedent.Symbols().Count;
                }
            }

            return agenda;
        }

        /// <summary>
        /// Validates that the query is a single symbol.
        /// </summary>
        private void ValidateHornQuery()
        {
            if (!(Base.Query is Symbol))
            {
                throw new ArgumentException("Query must be a single symbol.");
            }
        }
    }

}
