using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InferenceEngine
{
    public static class Parser
    {
        public static Dictionary<TokenType, string> Tokens = new Dictionary<TokenType, string>
        {
            { TokenType.SYMBOL, "" },
            { TokenType.LPAREN, "(" },
            { TokenType.RPAREN, ")" },
            { TokenType.NOT, "~" },
            { TokenType.AND, "&" },
            { TokenType.OR, "||" },
            { TokenType.IMPLIES, "=>" },
            { TokenType.BICONDITIONAL, "<=>" },
        };

        /*private static string Pattern { get { return string.Join("|", tokens.Select(pair => $"{pair.Value}")); } }*/
        private static string Pattern { get { return @"[A-Za-z][a-zA-Z0-9]*|&|\|\||=>|<=>|~|\(|\)"; } }

        public static List<(TokenType Type, string Value)> GetTokens(string input)
        {
            var matches = Regex.Matches(input, Pattern);
            List<(TokenType, string)> tokens = new List<(TokenType, string)>();
            foreach (Match match in matches)
            {
                TokenType tokenType = TokenType.SYMBOL; // if none matches, it must be symbols
                foreach (var tokenPair in Tokens)
                    if (match.Value == tokenPair.Value) 
                    {
                        tokenType = tokenPair.Key;
                        break;
                    }
                tokens.Add((tokenType, match.Value));
            }
            return tokens;
        }

        public static Clause Parse(List<(TokenType Type, string Value)> tokens)
        {
            var (expression, remainingTokens) = ParseBiconditional(tokens);
            if (remainingTokens.Count > 0)
            {
                throw new Exception("Unexpected tokens at the end of input");
            }
            return expression;
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseBiconditional(List<(TokenType Type, string Value)> tokens)
        {
            var (expression, remainingTokens) = ParseImplication(tokens);

            while (remainingTokens.Count > 0 && remainingTokens[0].Type == TokenType.BICONDITIONAL)
            {
                var rightExpression = ParseImplication(remainingTokens.GetRange(1, remainingTokens.Count - 1));
                expression = new Biconditional(expression, rightExpression.Expression);
                remainingTokens = rightExpression.RemainingTokens;
            }

            return (expression, remainingTokens);
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseImplication(List<(TokenType Type, string Value)> tokens)
        {
            var (expression, remainingTokens) = ParseDisjunction(tokens);

            while (remainingTokens.Count > 0 && remainingTokens[0].Type == TokenType.IMPLIES)
            {
                var rightExpression = ParseImplication(remainingTokens.GetRange(1, remainingTokens.Count - 1));
                expression = new Implication(expression, rightExpression.Expression);
                remainingTokens = rightExpression.RemainingTokens;
            }

            return (expression, remainingTokens);
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseDisjunction(List<(TokenType Type, string Value)> tokens)
        {
            var (expression, remainingTokens) = ParseConjunction(tokens);

            while (remainingTokens.Count > 0 && remainingTokens[0].Type == TokenType.OR)
            {
                var rightExpression = ParseConjunction(remainingTokens.GetRange(1, remainingTokens.Count - 1));
                expression = new Disjunction(expression, rightExpression.Expression);
                remainingTokens = rightExpression.RemainingTokens;
            }

            return (expression, remainingTokens);
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseConjunction(List<(TokenType Type, string Value)> tokens)
        {
            var (expression, remainingTokens) = ParseNegation(tokens);

            while (remainingTokens.Count > 0 && remainingTokens[0].Type == TokenType.AND)
            {
                var rightExpression = ParseNegation(remainingTokens.GetRange(1, remainingTokens.Count - 1));
                expression = new Conjunction(expression, rightExpression.Expression);
                remainingTokens = rightExpression.RemainingTokens;
            }

            return (expression, remainingTokens);
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseNegation(List<(TokenType Type, string Value)> tokens)
        {
            if (tokens.Count > 0 && tokens[0].Type == TokenType.NOT)
            {
                var innerExpression = ParseParentheses(tokens.GetRange(1, tokens.Count - 1));
                // Handle double negation
                if (innerExpression.Expression is Negation negation)
                {
                    return (negation.Arg, innerExpression.RemainingTokens);
                }
                return (new Negation(innerExpression.Expression), innerExpression.RemainingTokens);
            }

            return ParseParentheses(tokens);
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseParentheses(List<(TokenType Type, string Value)> tokens)
        {
            if (tokens.Count > 0 && tokens[0].Type == TokenType.LPAREN)
            {
                var innerExpression = ParseBiconditional(tokens.GetRange(1, tokens.Count - 1));

                if (innerExpression.RemainingTokens.Count > 0 && innerExpression.RemainingTokens[0].Type == TokenType.RPAREN)
                {
                    return (innerExpression.Expression, innerExpression.RemainingTokens.GetRange(1, innerExpression.RemainingTokens.Count - 1));
                }

                throw new Exception("Expected ')'");
            }

            return ParseSymbol(tokens);
        }

        private static (Clause Expression, List<(TokenType Type, string Value)> RemainingTokens) ParseSymbol(List<(TokenType Type, string Value)> tokens)
        {
            if (tokens.Count > 0 && tokens[0].Type == TokenType.SYMBOL)
            {
                var token = tokens[0];
                return (new Symbol(token.Value), tokens.GetRange(1, tokens.Count - 1));
            }

            throw new Exception("Expected a symbol");
        }
    }
}
