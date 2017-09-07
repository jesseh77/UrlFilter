using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionTypes;

namespace UrlFilter
{
    internal class ExpressionReducer
    {
        private readonly ExpressionOperator _operators;
        private readonly List<IExpressionProcessor> _processors;
        private readonly OperatorPrecedence _precedence;

        public ExpressionReducer(ExpressionOperator operators)
        {
            _operators = operators;
            _processors = GetExpressionProcessors();
            _precedence = new OperatorPrecedence();
        }

        private List<IExpressionProcessor> GetExpressionProcessors()
        {
            return new List<IExpressionProcessor>
            {
                new UnaryProcessor(new List<string>{"not"}),
                new ValueProcessor(new List<string>{"gt", "ge", "lt", "le"}, _operators),
                new ValueProcessor(new List<string>{"eq", "ne"}, _operators),
                new LogicalProcessor(new List<string>{"and"}, _operators),
                new LogicalProcessor(new List<string>{"or"}, _operators)
            };
        }

        internal Expression ReduceGroupSegments(IEnumerable<Token> tokens, ParameterExpression parameterExpression)
        {
            var currentTokens = tokens.ToList();
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

        private Expression ReduceExpressionSegments(LinkedList<Token> linkedList, ParameterExpression parameterExpression)
        {
            foreach (var processor in _processors)
            {
                processor.Process(linkedList, parameterExpression);
            }

            return linkedList.First.Value.OperatorExpression;
        }

        internal Expression ProcessQueryText<T>(string queryString, ParameterExpression parameterExpression)
        {
            var segments = SplitQueryTextSegments(queryString);
            var tokens = MapSegmentsToTokens(segments);
            return ReduceGroupSegments(tokens, parameterExpression);
        }

        private IEnumerable<Token> MapSegmentsToTokens(IEnumerable<string> segments)
        {
            return segments.Select(x => new Token(x));
        }

        private static IEnumerable<string> SplitQueryTextSegments(string queryString)
        {
            int segmentStart = 0;
            char segmentDelimiter = ' ';
            int length = queryString.Length;
            
            for (int i = 0; i < length; i++)
            {
                var currentCharacter = queryString[i];
                
                if (currentCharacter == '(')
                {
                    segmentDelimiter = ' ';
                    segmentStart = i + 1;
                    yield return currentCharacter.ToString();
                }

                if (currentCharacter == ')')
                {
                    segmentDelimiter = ' ';
                    yield return queryString.Substring(segmentStart, i - segmentStart);
                    yield return currentCharacter.ToString();
                    segmentStart = i + 1;
                }

                if (currentCharacter == segmentDelimiter)
                {
                    var segment = queryString.Substring(segmentStart, i - segmentStart);
                    segmentDelimiter = ' ';
                    if(segment.Length != 0)
                    {
                        yield return segment;
                    }
                    segmentStart = i + 1;
                    continue;
                }

                if(currentCharacter == '\'')
                {
                    segmentStart = i + 1;
                    segmentDelimiter = currentCharacter;
                }

                if (currentCharacter == ' ')
                {
                    if(segmentDelimiter == '\'') { continue; }
                    segmentStart = i + 1;
                    segmentDelimiter = currentCharacter;
                }

                if (i == length - 1)
                {
                    yield return queryString.Substring(segmentStart);
                }
            }
        }
    }
}
