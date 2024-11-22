using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class ForwardChaining: IEngine
    {
        public ForwardChaining(KnowledgeBase knowledgeBase)
        {
            Base = knowledgeBase;
            ValidateHornQuery();
        }

        public KnowledgeBase Base { get; set; }
        public bool Entails { get; set; }
        public List<Symbol> InferenceChain { get; set; }

        public void Solve()
        {
            // Initialize inferred and count dictionaries
            var inferred = Base.Base
                .SelectMany(clause => clause.Symbols())
                .Distinct()
                .ToDictionary(symbol => symbol, _ => false);

            Dictionary<Implication, int> count = new Dictionary<Implication, int>();

            // Initialize the agenda with symbols known to be true
            Queue<Symbol> agenda = InitializeAgenda(Base.Base, count);

            // Track the result of forward chaining
            List<Symbol> chain = new List<Symbol>();

            while (agenda.Count > 0)
            {
                // Dequeue the first symbol
                Symbol p = agenda.Dequeue();
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

                    foreach (Implication clause in Base.Base.OfType<Implication>())
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

        private Queue<Symbol> InitializeAgenda(List<Clause> baseClauses, Dictionary<Implication, int> count)
        {
            Queue<Symbol> agenda = new Queue<Symbol>();

            foreach (Clause clause in baseClauses)
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

        private void ValidateHornQuery()
        {
            if (!(Base.Query is Symbol))
            {
                throw new ArgumentException("Query must be a single symbol.");
            }
        }

        public void PrintResult()
        {
            Console.WriteLine((Entails) ?
                                $"YES: {string.Join(", ", InferenceChain)}" :
                                "NO");
        }
    }

}
