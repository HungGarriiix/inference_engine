using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public enum TokenType
    {
        SYMBOL,
        LPAREN,
        RPAREN,
        NOT,
        AND,
        OR,
        IMPLIES,
        BICONDITIONAL
    }
}
