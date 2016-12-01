using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public static class QueryString<T> where T : class
    {        
        public static Expression<Func<T,bool>> GetWhereExpression(string queryString)
        {
            var tokens = GetWhereSegments(queryString);
            var parameterExpression = Expression.Parameter(typeof(T));

            var expression = ReduceGroupSegments(tokens, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        private static Expression ReduceGroupSegments(List<Token> tokens, ParameterExpression parameterExpression)
        {
            var currentTokens = tokens;
            while (currentTokens.Count != 1)
            {
                currentTokens = ProcessGroupPriority(currentTokens, parameterExpression);
            }
            return currentTokens.Single().OperatorExpression;
        }

        private static List<Token> ProcessGroupPriority(List<Token> tokens, ParameterExpression parameterExpression)
        {
            var leftBracket = 0;

            for (int i = 0; i < tokens.Count -1; i++)
            {
                var token = tokens[i];
                if (token.GroupPriority == OperatorPrecedence.Precedence.Grouping)
                {
                    if(token.TokenValue == "(")
                    { 
                        leftBracket = i;
                    }
                    else if(token.TokenValue == ")")
                    {
                        var groupTokens = tokens.Skip(leftBracket + 1).Take(i - leftBracket - 1).ToList();
                        var groupExpression = ReduceExpressionSegments(new LinkedList<Token>(groupTokens), parameterExpression);
                        tokens.RemoveRange(leftBracket, i - leftBracket + 1);
                        tokens.Insert(leftBracket, new Token { OperatorExpression = groupExpression });
                        return tokens;
                    }
                }
            }
            return new List<Token> { new Token { OperatorExpression = ReduceExpressionSegments(new LinkedList<Token>(tokens), parameterExpression) } };
        }

        private static Expression ReduceExpressionSegments(LinkedList<Token> tokens, ParameterExpression parameterExpression)
        {
            ExpressionProcessors.ProcessUnary(tokens, OperatorPrecedence.Precedence.Unary);
            ExpressionProcessors.ProcessEqualityAndRelational<T>(tokens, OperatorPrecedence.Precedence.Relational, parameterExpression);
            ExpressionProcessors.ProcessEqualityAndRelational<T>(tokens, OperatorPrecedence.Precedence.Equality, parameterExpression);
            ExpressionProcessors.ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalAnd);
            ExpressionProcessors.ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalOr);

            return tokens.First.Value.OperatorExpression;
        }

        private static List<Token> GetWhereSegments(string queryString)
        {
            var querySegments = PadBracketsWithSpace(queryString).Split(' ');
            var tokens = querySegments
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new Token
                {
                    GroupPriority = OperatorPrecedence.GetOperatorPrecedence(x),
                    TokenValue = x
                }).ToList();
            
            return tokens;
        }

        private static string PadBracketsWithSpace(string queryString)
        {
            return queryString.Replace("(", " ( ").Replace(")", " ) ");
        }
    }
}
