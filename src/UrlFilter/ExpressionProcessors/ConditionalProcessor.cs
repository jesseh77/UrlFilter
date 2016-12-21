using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    internal class ConditionalProcessor : IExpressionProcessor
    {
        private readonly ExpressionOperator _operators;

        public OperatorPrecedence.Precedence Precedence { get; }

        public ConditionalProcessor(OperatorPrecedence.Precedence precedence, ExpressionOperator operators)
        {
            Precedence = precedence;
            _operators = operators;
        }
        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (current.Next.Value.GroupPriority == Precedence)
                {
                    current.Next.Value.OperatorExpression =
                        _operators.OperatorExpression(current.Next.Value.TokenValue,
                            current.Value.OperatorExpression, current.Next.Next.Value.OperatorExpression);

                    tokens.Remove(current.Next.Next);
                    var next = current.Next;
                    tokens.Remove(current);
                    current = next;
                }
                else
                {
                    current = current.Next;
                }
            }
        }
    }
}
