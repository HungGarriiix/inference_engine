using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class ForwardChaining
{
    private readonly KnowledgeBase KnowledgeBase;
    private readonly Symbol Query;

    /// <summary>
    /// Initializes a new instance of the ForwardChaining solver.
    /// </summary>
    /// <param name="knowledgeBase">The knowledge base containing clauses and the query.</param>
    public ForwardChaining(KnowledgeBase knowledgeBase)
    {
        KnowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
        Query = knowledgeBase.Query ?? throw new ArgumentNullException("Query cannot be null.");
        ValidateHornKnowledgeBase();
        ValidateHornQuery();
    }

    /// <summary>
    /// Solves the query using Forward Chaining.
    /// </summary>
    /// <returns>A ForwardChainingResult object containing whether the query is entailed and the inference chain.</returns>
    public ForwardChainingResult Solve()
    {
        // Initialize inferred and count dictionaries
        var inferred = KnowledgeBase.Base
            .SelectMany(clause => clause.GetSymbols())
            .Distinct()
            .ToDictionary(symbol => symbol, _ => false);

        var count = new Dictionary<Implication, int>();

        // Initialize the agenda with symbols known to be true
        var agenda = InitializeAgenda(KnowledgeBase.Base, count);

        // Track the result of forward chaining
        var chain = new List<Symbol>();

        while (agenda.Count > 0)
        {
            // Dequeue the first symbol
            var p = agenda.Dequeue();
            chain.Add(p);

            // If the query is found, return the result
            if (p == Query)
            {
                return new ForwardChainingResult
                {
                    Entails = true,
                    InferenceChain = chain
                };
            }

            // Mark symbol as inferred and propagate to implications
            if (!inferred[p])
            {
                inferred[p] = true;

                foreach (var clause in KnowledgeBase.Base.OfType<Implication>())
                {
                    if (clause.Antecedent.GetSymbols().Contains(p))
                    {
                        count[clause]--;

                        if (count[clause] == 0)
                        {
                            agenda.Enqueue(clause.Consequent);
                        }
                    }
                }
            }
        }

        return new ForwardChainingResult
        {
            Entails = false,
            InferenceChain = chain
        };
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
                count[implication] = implication.Antecedent.GetSymbols().Count;
            }
        }

        return agenda;
    }

    /// <summary>
    /// Validates that the knowledge base is in Horn form.
    /// </summary>
    private void ValidateHornKnowledgeBase()
    {
        foreach (var clause in KnowledgeBase.Base)
        {
            if (!clause.IsHornClause())
            {
                throw new ArgumentException("Knowledge base must consist of Horn clauses.");
            }
        }
    }

    /// <summary>
    /// Validates that the query is a single symbol.
    /// </summary>
    private void ValidateHornQuery()
    {
        if (!(Query is Symbol))
        {
            throw new ArgumentException("Query must be a single symbol.");
        }
    }
}

}
