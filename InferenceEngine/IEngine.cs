using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public interface IEngine
    {
        KnowledgeBase Base { get; set; }
        bool Entails { get; set; }

        void Solve();
        void PrintResult();
    }
}
