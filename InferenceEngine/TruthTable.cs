using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class TruthTable
    {
        public KnowledgeBase KnowledgeBase { get; set; }
        public List<Symbol> Symbols { get; set; }
        public List<(Dictionary<Symbol, bool>, bool?, bool?)> Table { get; set; }
        public int ValidModelsCount { get; set; }

        public TruthTable(KnowledgeBase knowledgeBase)
        {
            KnowledgeBase = knowledgeBase;
            Symbols = KnowledgeBase.Base
                        .SelectMany(clause => clause.Symbols())
                        .Union(KnowledgeBase.Query.Symbols())
                        .Distinct()
                        .OrderBy(symbol => symbol.Name)
                        .ToList();
            Table = new List<(Dictionary<Symbol, bool>, bool?, bool?)>();
            ValidModelsCount = 0;
        }

        public (bool Entails, int? ValidModelCount) Solve()
        {
            if (KnowledgeBase.Query == null)
                throw new InvalidOperationException("No query provided in the knowledge base.");

            var model = new Dictionary<Symbol, bool>();
            var valid = CheckAll(KnowledgeBase.Base, KnowledgeBase.Query, Symbols, model);

            if (valid && ValidModelsCount > 0)
            {
                return (true, ValidModelsCount);
            }

            return (false, null);
        }

        private bool CheckAll(List<Clause> baseClauses, Clause query, List<Symbol> symbols, Dictionary<Symbol, bool> model)
        {
            if (!symbols.Any())
            {
                // Evaluate the knowledge base (KB) and query
                bool? kbEval = EvaluateClauses(baseClauses, model);
                bool? queryEval = query.Evaluate(model);

                // Handle null results
                if (kbEval == null || queryEval == null)
                {
                    Table.Add((new Dictionary<Symbol, bool>(model), kbEval, queryEval));
                    return true; // Allow checking further models in case of null.
                }

                // Store this model in the truth table
                Table.Add((new Dictionary<Symbol, bool>(model), kbEval, queryEval));

                if (kbEval == true && queryEval == true)
                {
                    ValidModelsCount++;
                }

                return kbEval == true ? queryEval == true : true;
            }
            else
            {
                // Split the symbols list
                var symbol = symbols.First();
                var rest = symbols.Skip(1).ToList();

                // Check all models with the current symbol set to true and false
                return CheckAll(baseClauses, query, rest, ExtendModel(model, symbol, true)) &&
                       CheckAll(baseClauses, query, rest, ExtendModel(model, symbol, false));
            }
        }

        private Dictionary<Symbol, bool> ExtendModel(Dictionary<Symbol, bool> model, Symbol symbol, bool value)
        {
            var newModel = new Dictionary<Symbol, bool>(model)
            {
                [symbol] = value
            };
            return newModel;
        }

        private bool? EvaluateClauses(List<Clause> clauses, Dictionary<Symbol, bool> model)
        {
            // Evaluate all clauses in the knowledge base. Return null if any evaluation is null.
            foreach (var clause in clauses)
            {
                bool? clauseEval = clause.Evaluate(model);
                if (clauseEval == null)
                {
                    return null;
                }
                if (clauseEval == false)
                {
                    return false; // Short-circuit evaluation for KB.
                }
            }
            return true;
        }

        public string GenerateTable()
        {
            if (!Table.Any())
            {
                Solve();
            }

            // Create table headers
            var headers = Symbols.Select(symbol => symbol.Name).ToList();
            headers.Add("KB");
            headers.Add("Query: " + KnowledgeBase.Query.ToString());

            // Generate table rows
            var rows = new List<List<string>>();
            foreach (var (model, kbEval, queryEval) in Table)
            {
                var row = Symbols.Select(symbol => model[symbol].ToString())
                                 .ToList();
                row.Add(kbEval?.ToString() ?? "null");
                row.Add(queryEval?.ToString() ?? "null");
                rows.Add(row);
            }

            // Format the table as a string
            return FormatTable(headers, rows);
        }

        private string FormatTable(List<string> headers, List<List<string>> rows)
        {
            int[] columnWidths = headers.Select(header => header.Length).ToArray();
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], row[i].Length);
                }
            }

            string separator = string.Join("+", columnWidths.Select(w => new string('-', w)));
            string headerRow = string.Join("|", headers.Select((h, i) => h.PadRight(columnWidths[i])));
            string table = separator + "\n" + headerRow + "\n" + separator + "\n";

            foreach (var row in rows)
            {
                table += string.Join("|", row.Select((cell, i) => cell.PadRight(columnWidths[i]))) + "\n";
            }

            table += separator;
            return table;
        }
    }
}
