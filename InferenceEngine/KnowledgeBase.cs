using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public class KnowledgeBase
    {
        public KnowledgeBase(string file) 
        {
            Base = new List<Clause>();
            Query = null;
            IsExpected = null;
            ReadFile(file);
            PrintOutlines();
        }

        public List<Clause> Base { get; set; }
        public Clause Query { get; set; }
        public bool? IsExpected { get; set; }

        public HashSet<Symbol> Symbols 
        { 
            get 
            {
                HashSet<Symbol> result = new HashSet<Symbol>();
                foreach (Clause clause in Base)
                    foreach (Symbol symbol in clause.Symbols())
                        result.Add(symbol);
                return result;
            } 
        }
        public void ReadFile(string file)
        {
            StreamReader sr = new StreamReader(file);

            while(!sr.EndOfStream) 
            {
                string tell = sr.ReadLine();
                if (tell == null && tell != "TELL") throw new ArgumentException("Missing TELL.");

                // Knowledge base
                string baseRaw = sr.ReadLine();
                if (baseRaw == null) throw new ArgumentException("Missing Knowledge Base.");
                string[] baseSplit = baseRaw.Replace(" ", string.Empty).Split(';');
                foreach(string line in baseSplit)
                {
                    string lineSave = line.Trim();
                    if (lineSave == "")
                    {
                        continue;
                    }

                    var baseTokens = Parser.GetTokens(lineSave);
                    Clause baseClause = Parser.Parse(baseTokens);
                    Base.Add(baseClause);
                }

                string ask = sr.ReadLine();
                if (ask == null && ask != "ASK") throw new ArgumentException("Missing ASK.");

                // Query
                string queryRaw = sr.ReadLine();
                if (queryRaw == null) throw new ArgumentException("Missing Query.");
                var queryToken = Parser.GetTokens(queryRaw);
                Clause queryClause = Parser.Parse(queryToken);
                Query = queryClause;

                // Expect result (Optional)
                string expect = sr.ReadLine();
                if (expect != null)
                {
                    if (expect != "EXPECT") throw new ArgumentException("Missing EXPECT.");

                    string result = sr.ReadLine();
                    if (result != null && (result != "YES" || result != "NO"))
                        IsExpected = (result == "YES") ? true : false;
                }
            }
        }

        public void PrintOutlines()
        {
            Console.WriteLine("Knowledge base:\n");
            foreach (Clause clause in Base) Console.WriteLine(clause.ToString());

            Console.WriteLine($"Query: {Query.ToString()}\n");
            Console.WriteLine((IsExpected == null) ? "" : $"Expecting: {((IsExpected == true) ? "YES" : "NO")}");
        }
    }
}
