using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    internal class ExpressionReducer
    {
        private ExpressionProcessors _processors;

        public ExpressionReducer(ExpressionProcessors processors)
        {
            _processors = processors;
        }

        internal Expression ReduceGroupSegments<T>(List<Token> tokens, ParameterExpression parameterExpression)
        {
            var currentTokens = tokens;
            while (currentTokens.Count != 1)
            {
                currentTokens = ProcessGroupPriority<T>(currentTokens, parameterExpression);
            }
            return currentTokens.Single().OperatorExpression;
        }

        private List<Token> ProcessGroupPriority<T>(List<Token> tokens, ParameterExpression parameterExpression)
        {
            var leftBracket = 0;

            for (var i = 0; i < tokens.Count - 1; i++)
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
                    var groupExpression = ReduceExpressionSegments<T>(new LinkedList<Token>(groupTokens), parameterExpression);
                    tokens.RemoveRange(leftBracket, i - leftBracket + 1);
                    tokens.Insert(leftBracket, new Token { OperatorExpression = groupExpression });
                    return tokens;
                }
            }
            return new List<Token> { new Token { OperatorExpression = ReduceExpressionSegments<T>(new LinkedList<Token>(tokens), parameterExpression) } };
        }

        private Expression ReduceExpressionSegments<T>(LinkedList<Token> tokens, ParameterExpression parameterExpression)
        {
            _processors.ProcessUnary(tokens, OperatorPrecedence.Precedence.Unary);
            _processors.ProcessEqualityAndRelational<T>(tokens, OperatorPrecedence.Precedence.Relational, parameterExpression);
            _processors.ProcessEqualityAndRelational<T>(tokens, OperatorPrecedence.Precedence.Equality, parameterExpression);
            _processors.ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalAnd);
            _processors.ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalOr);

            return tokens.First.Value.OperatorExpression;
        }
    }
}
