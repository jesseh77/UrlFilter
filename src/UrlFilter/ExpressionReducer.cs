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

        private IExpressionProcessor[] GetExpressionProcessors()
        {
            return new IExpressionProcessor[]
            {
                new UnaryProcessor(),
                new EqualityAndRelationalProcessor(OperatorPrecedence.Precedence.Relational, _operators),
                new EqualityAndRelationalProcessor(OperatorPrecedence.Precedence.Equality, _operators),
                new ConditionalProcessor(OperatorPrecedence.Precedence.ConditionalAnd, _operators),
                new ConditionalProcessor(OperatorPrecedence.Precedence.ConditionalOr, _operators)
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
                if (token.GroupPriority != OperatorPrecedence.Precedence.Grouping) continue;

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

        private Expression ReduceExpressionSegments(LinkedList<Token> tokens, ParameterExpression parameterExpression)
        {
            foreach (var expressionProcessor in _processors)
            {
                expressionProcessor.Process(tokens, parameterExpression);
            }
            return tokens.First.Value.OperatorExpression;
        }

        internal List<Token> GetWhereSegments(string queryString, ICollection<string> customOperators = null)
        {
            var querySegments = PadBracketsWithSpace(queryString).Split(' ');
            var tokens = querySegments.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new Token
                {
                    GroupPriority = _precedence.GetOperatorPrecedence(x, customOperators),
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
