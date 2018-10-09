using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionTypes;

namespace UrlFilter
{
    public class ExpressionReducer
    {
        private readonly List<IExpressionProcessor> _processors;
        private readonly OperatorPrecedence _precedence;

        public ExpressionReducer()
        {
            _processors = GetExpressionProcessors();
            _precedence = new OperatorPrecedence();
        }

        public List<IExpressionProcessor> GetExpressionProcessors()
        {
            return new IExpressionProcessor[]
            {
                new UnaryProcessor("not", Expression.Not),
                new ValueProcessor("gt", Expression.GreaterThan),
                new ValueProcessor("ge", Expression.GreaterThanOrEqual),
                new ValueProcessor("lt", Expression.LessThan),
                new ValueProcessor("le", Expression.LessThanOrEqual),
                new ValueProcessor("eq", Expression.Equal),
                new ValueProcessor("ne", Expression.NotEqual),
                new LogicalProcessor("and", Expression.AndAlso),
                new LogicalProcessor("or", Expression.OrElse)
            }
            .OrderBy(x => x.ExpressionCategory)
            .ToList();
        }

        public Expression ReduceGroupSegments(List<Token> tokens, ParameterExpression parameterExpression)
        {
            while (tokens.Count != 1)
            {
                tokens = ProcessGroupPriority(tokens, parameterExpression);
            }
            return tokens.Single().OperatorExpression;
        }

        public List<Token> ProcessGroupPriority(List<Token> tokens, ParameterExpression parameterExpression)
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

        public Expression ReduceExpressionSegments(LinkedList<Token> linkedList, ParameterExpression parameterExpression)
        {
            foreach (var processor in _processors)
            {
                processor.Process(linkedList, parameterExpression);
            }

            return linkedList.First.Value.OperatorExpression;
        }

        public Expression ProcessQueryText<T>(string queryString, ParameterExpression parameterExpression)
        {
            var segments = SplitQueryTextSegments(queryString);
            var tokens = MapSegmentsToTokens(segments);
            return ReduceGroupSegments(tokens, parameterExpression);
        }

        public List<Token> MapSegmentsToTokens(IEnumerable<string> segments)
        {
            return segments.Select(x => new Token(x)).ToList();
        }

        public IEnumerable<string> SplitQueryTextSegments(string queryString)
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
