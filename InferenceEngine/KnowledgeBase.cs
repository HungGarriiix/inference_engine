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
            ReadFile(file);
        }

        public List<Clause> Base { get; set; }
        public Clause Query { get; set; }

        public void ReadFile(string file)
        {
            StreamReader sr = new StreamReader(file);

            while(!sr.EndOfStream) 
            {
                sr.ReadLine();  // skip TELL
                string baseRaw = sr.ReadLine();
                baseRaw = baseRaw.Replace(" ", string.Empty);
                string[] baseSplit = baseRaw.Split(';');
                foreach(string line in baseSplit)
                {
                    string lineSave = line.Trim();
                    Console.WriteLine(lineSave);
                    if (lineSave == "")
                    {
                        continue;
                    }

                    var baseTokens = Parser.GetTokens(lineSave);
                    Clause baseClause = Parser.Parse(baseTokens);
                    Console.WriteLine(baseClause.ToString());
                    Base.Add(baseClause);
                }

                sr.ReadLine(); // skip ASK
                string queryRaw = sr.ReadLine();
                var queryToken = Parser.GetTokens(queryRaw);
                Clause queryClause = Parser.Parse(queryToken);

                Console.WriteLine($"ASK: {queryClause.ToString()}");
                Query = queryClause;
            }
        }

    }
}
