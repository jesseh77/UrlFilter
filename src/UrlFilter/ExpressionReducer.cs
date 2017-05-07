using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter
{
    internal class ExpressionReducer
    {
        private readonly ExpressionOperator _operators;
        private readonly IExpressionProcessor[] _processors;
        private readonly OperatorPrecedence _precedence;

        public ExpressionReducer(ExpressionOperator operators)
        {
            _operators = operators;
            _processors = GetExpressionProcessors();
            _precedence = new OperatorPrecedence();
        }

        private List<IExpressionProcessor> IExpressionProcessor GetExpressionProcessors()
        {
            return new List<IExpressionProcessor>
            {
                new UnaryProcessor(new List<string>{"not"}),
                new ValueProcessor(new List<string>{"gt", "ge", "lt", "le"}, _operators),
                new ValueProcessor(new List<string>{"eq", "ne"}, _operators),
                new ExpressionProcessor(new List<string>{"and"}, _operators),
                new ExpressionProcessor(new List<string>{"or"}, _operators)
            };
        }



        internal Expression ReduceGroupSegments(List<Token> tokens, ParameterExpression parameterExpression)
        {
            var currentTokens = tokens;
            while (currentTokens.Count != 1)
            {
                currentTokens = ProcessGroupPriority(currentTokens, parameterExpression);
            }
            return currentTokens.Single().OperatorExpression;
        }

        private List<Token> ProcessGroupPriority(List<Token> tokens, ParameterExpression parameterExpression)
        {
            var leftBracket = 0;

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                
                if (token.TokenValue == "(")
                {
                    leftBracket = i;
                }
                else if (token.TokenValue == ")")
                {
                    var groupTokens = tokens.Skip(leftBracket + 1).Take(i - leftBracket - 1).ToList();
                    var groupExpression = ReduceExpressionSegments(new LinkedList<Token>(groupTokens), parameterExpression);
                    tokens.RemoveRange(leftBracket, i - leftBracket + 1);
                    tokens.Insert(leftBracket, new Token { OperatorExpression = groupExpression });
                    return tokens;
                }
            }
            return new List<Token> { new Token { OperatorExpression = ReduceExpressionSegments(new LinkedList<Token>(tokens), parameterExpression) } };
        }

        internal Expression ProcessQueryText<T>(string queryString, ParameterExpression parameterExpression)
        {
            return ProcessQueryText<T>(queryString, parameterExpression, new Dictionary<string, Func<string, object, Expression>>());
        }

        internal Expression ProcessQueryText<T>(string queryString, ParameterExpression parameterExpression, IDictionary<string, Func<string, object, Expression>> customExpressions)
        {
            var expresionProcessors = GetExpressionProcessors();
            var segments = SplitQueryTextSegments(queryString);
            var tokens = MapSegmentsToTokens(segments, customExpressions);
            
        }

        private List<Token> MapSegmentsToTokens(List<string> segments)
        {

        }

        private static List<string> SplitQueryTextSegments(string queryString)
        {
            return queryString
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
